using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Adminpage : pages.Basepage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDropdown();
            }

            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/pages/Mainpage.aspx");
            }
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            ClearErrorMessages();
            string username = txbUsername.Text;
            string password = txbPassword.Text;
            bool isAdmin = chkIsAdmin.Checked;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessageCreateUser.InnerText = "Brugernavn og password skal udfyldes";
                return;
            }
            if (Lib.Managers.UserManager.Current.IsUsernameTaken(username))
            {
                ErrorMessageCreateUser.InnerText = "Brugernavnet er optaget";
                return;
            }
            Lib.Managers.UserManager.Current.CreateUser(username, password, isAdmin);
            PopulateDropdown();
            ClearErrorMessages();
            ClearFields();
        }

        protected void btnReturnToMainpage_Click(object sender, EventArgs e)
        {
            ClearErrorMessages();
            ClearFields();
            Response.Redirect("/pages/Mainpage.aspx");
        }

        protected void drpUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }

        protected void btnUpdateCurrentUser_Click(object sender, EventArgs e)
        {
            string newName = txtCurrentUserName.Text;
            bool newIsAdmin = chkCurrentUserIsAdmin.Checked;
            User selectedUser = Lib.Managers.UserManager.Current.GetUserById(int.Parse(drpUsers.SelectedValue));
            if (string.IsNullOrWhiteSpace(newName))
            {
                ErrorMessageUpdateUser.InnerText = "Navnet må ikke være tomt";
                return;
            }
            if (Lib.Managers.UserManager.Current.IsUsernameTaken(newName) && !selectedUser.Name.Equals(newName))
            {
                ErrorMessageUpdateUser.InnerText = "Navnet er allerede taget";
                return;
            }
            selectedUser.Name = txtCurrentUserName.Text;
            selectedUser.IsAdmin = chkCurrentUserIsAdmin.Checked;

            Lib.Managers.UserManager.Current.UpdateUser(selectedUser);

            PopulateDropdown();
            ClearErrorMessages();
        }

        protected void btnDeleteCurrentUser_Click(object sender, EventArgs e)
        {
            User selectedUser = Lib.Managers.UserManager.Current.GetUserById(int.Parse(drpUsers.SelectedValue));
            selectedUser.Deleted = true;

            Lib.Managers.UserManager.Current.UpdateUser(selectedUser);

            PopulateDropdown();
            ClearErrorMessages();
        }

        private void SelectionChanged()
        {
            User selectedUser = Lib.Managers.UserManager.Current.GetUserById(int.Parse(drpUsers.SelectedValue));
            txtCurrentUserName.Text = selectedUser.Name;
            chkCurrentUserIsAdmin.Checked = selectedUser.IsAdmin;
        }

        private void ClearErrorMessages()
        {
            ErrorMessageCreateUser.InnerText = "";
            ErrorMessageUpdateUser.InnerText = "";
        }

        private void ClearFields()
        {
            txbPassword.Text = "";
            txbUsername.Text = "";
            chkIsAdmin.Checked = false;

            txtCurrentUserName.Text = "";
            chkCurrentUserIsAdmin.Checked = false;
        }

        /// <summary>
        /// Rebinds user data for the dropdown
        /// </summary>
        private void PopulateDropdown()
        {
            List<User> users = Lib.Managers.UserManager.Current.GetAllUsers();
            drpUsers.DataSource = users;
            drpUsers.DataTextField = "Name";
            drpUsers.DataValueField = "Id";

            drpUsers.DataBind();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
    }
}