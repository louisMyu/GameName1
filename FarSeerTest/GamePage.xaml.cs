using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using FarSeerTest.Resources;

namespace FarSeerTest
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Game1 _game;

        // Constructor
        public GamePage()
        {
            InitializeComponent();

            _game = XamlGame<Game1>.Create("", this);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void LeftButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _game.LeftButton();
        }
        private void RightButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _game.RightButton();
        }
        private void ResetButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _game.ResetButton();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}