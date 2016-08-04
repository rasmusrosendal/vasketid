using Lib.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string name = txbUsername.Text;
            string password = txbPassword.Text;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage.InnerText = "Navn og password må ikke være tomt";
                return;
            }
            Login();
        }

        private void Login(User user = null)
        {
            User currentUser;

            if (user == null)
            {
                string username = txbUsername.Text;
                string password = txbPassword.Text;
                currentUser = Lib.Managers.UserManager.Current.ValidateLogin(username, password); 
            }
            else
            {
                currentUser = user;
            }

            if (currentUser != null)
            {
                Response.Cookies["VasketidUserId"].Value = currentUser.Id.ToString();
                Response.Cookies["VasketidUserId"].Expires = DateTime.Now.AddYears(50);
                Response.Redirect("/pages/Mainpage.aspx");
            }
            else
            {
                errorMessage.InnerText = "Forkert brugernavn eller password";
            }
        }

        protected void btnNewUser_Click(object sender, EventArgs e)
        {
            string name = txbUsername.Text;
            string password = txbPassword.Text;
            if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage.InnerText = "Navn og password må ikke være tomt";
                return;
            }
            if (Lib.Managers.UserManager.Current.IsUsernameTaken(name))
            {
                errorMessage.InnerText = "Navnet er optaget, vælg et andet";
                return;
            }

            User createdUser = Lib.Managers.UserManager.Current.CreateUser(name, password, false);
            Login(createdUser);
        }
    }
}