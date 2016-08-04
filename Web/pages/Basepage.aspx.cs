using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.pages
{
    public partial class Basepage : System.Web.UI.Page
    {
        private User mCurrentUser;
        public User CurrentUser
        {
            get
            {
                if (mCurrentUser == null)
                {
                    if (Request.Cookies["VasketidUserId"] != null)
                    {
                        HttpCookie cookie = Request.Cookies["VasketidUserId"];
                        mCurrentUser = Lib.Managers.UserManager.Current.GetUserById(int.Parse(cookie.Value));
                    }
                    else
                    {
                        Response.Redirect("/Default.aspx");
                    }
                }
                return mCurrentUser;
            }
        }

        public Basepage()
        {
            this.Load += new EventHandler(Base_Page_Load);
        }

        protected void Base_Page_Load(object sender, EventArgs e)
        {
            if(Request.Cookies["VasketidUserId"] == null)
            {
                Response.Redirect("/Default.aspx");
            }
        }

        public void Logout()
        {
            if (Request.Cookies["VasketidUserId"] != null)
            {
                HttpCookie currentCookie = Request.Cookies["VasketidUserId"];
                currentCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(currentCookie);
            }

            Response.Redirect("/Default.aspx");
        }
    }
}