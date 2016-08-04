using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Welcome : pages.Basepage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pWelcomeUser.InnerText = "Velkommen, " + CurrentUser.Name;
                btnGoToAdminpage.Visible = CurrentUser.IsAdmin;
                InitiateDropDownLists();
            }

            PopulateBookings();
        }

        private void InitiateDropDownLists()
        {
            List<ListItem> ddlFromSource = new List<ListItem>();
            for (int i = 9; i < 20; i += 2)
            {
                ddlFromSource.Add(new ListItem(i + ":00", i.ToString()));
            }
            ddlFrom.DataValueField = "Value";
            ddlFrom.DataTextField = "Text";
            ddlFrom.DataSource = ddlFromSource;
            ddlFrom.DataBind();

            List<ListItem> ddlToSource = new List<ListItem>();
            for (int i = 11; i < 22; i += 2)
            {
                ddlToSource.Add(new ListItem(i + ":00", i.ToString()));
            }
            ddlTo.DataValueField = "Value";
            ddlTo.DataTextField = "Text";
            ddlTo.DataSource = ddlToSource;
            ddlTo.DataBind();
        }

        private void PopulateBookings()
        {
            try
            {
                DateTime selectedDate = DateTime.Parse(datepicker.Value);

                DataTable dt = new DataTable();
                dt.Columns.Add("Time", typeof(string));
                dt.Columns.Add("Bookings", typeof(string));
                dt.Columns.Add("Note", typeof(string));

                DataRow dr;
                List<Booking> bookings = Lib.Managers.BookingManager.Current.GetBookingsByDate(selectedDate);

                for (int i = 9; i < 21; i += 2)
                {
                    dr = dt.NewRow();
                    dr["Time"] = i == 9 ? "0" + i + ".00" : i.ToString() + ".00";
                    Booking b = bookings.Where(x => x.Time.Hour == i).FirstOrDefault();
                    if (b != null)
                    {
                        dr["Bookings"] = Lib.Managers.UserManager.Current.GetUserById(b.UserId).Name;
                        dr["Note"] = b.Note;
                    }
                    else
                    {
                        dr["Bookings"] = "";
                    }

                    dt.Rows.Add(dr);
                }

                grvBookings.DataSource = dt;
                grvBookings.DataBind();

                ShowBookingsDate.InnerText = "Viser bookinger for: " + DateTimeFormatInfo.CurrentInfo.GetDayName(selectedDate.DayOfWeek) + " den " + selectedDate.Day + "/" + selectedDate.Month;
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            ClearErrorMessage();
            DateTime selectedDate = new DateTime();
            try
            {
                selectedDate = DateTime.Parse(datepicker.Value);
            }
            catch (Exception)
            {
                ErrorMessage.InnerText = "Vælg en dato";
                return;
            }
            DateTime from = selectedDate.AddHours(int.Parse(ddlFrom.SelectedValue));
            DateTime to = selectedDate.AddHours(int.Parse(ddlTo.SelectedValue));
            string note = txbNote.Text;

            if (from < to)
            {
                try
                {
                    if (Lib.Managers.BookingManager.Current.CheckAvailability(from, to))
                    {
                        Lib.Managers.BookingManager.Current.CreateBooking(from, to, CurrentUser.Id, note);
                    }
                    else
                    {
                        ErrorMessage.InnerText = "Tidspunktet er allerede booket. Vælg et andet.";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.InnerText = "Kunne ikke foretage booking";
                }
            }
            else
            {
                ErrorMessage.InnerText = "Sluttidspunkt skal være senere end starttidspunkt";
            }
            PopulateBookings();
            ClearNote();
        }

        private void ClearErrorMessage()
        {
            ErrorMessage.InnerText = "";
        }

        private void ClearNote()
        {
            txbNote.Text = "";
        }

        protected void btnDeleteBookings_Click(object sender, EventArgs e)
        {
            ClearErrorMessage();
            DateTime selectedDate = new DateTime();
            try
            {
                selectedDate = DateTime.Parse(datepicker.Value);
            }
            catch (Exception)
            {
                ErrorMessage.InnerText = "Vælg en dato";
                return;
            }
            DateTime from = selectedDate.AddHours(int.Parse(ddlFrom.SelectedValue));
            DateTime to = selectedDate.AddHours(int.Parse(ddlTo.SelectedValue));

            if (from < to)
            {
                try
                {
                    Lib.Managers.BookingManager.Current.DeleteBooking(from, to, CurrentUser.Id);
                }
                catch (Exception)
                {
                    ErrorMessage.InnerText = "Kunne ikke slette booking";
                }
            }
            else
            {
                ErrorMessage.InnerText = "Sluttidspunkt skal være senere end starttidspunkt";
            }
            PopulateBookings();
        }

        protected void btnGoToAdminpage_Click(object sender, EventArgs e)
        {
            if (CurrentUser.IsAdmin)
            {
                Response.Redirect("/pages/Adminpage.aspx");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}