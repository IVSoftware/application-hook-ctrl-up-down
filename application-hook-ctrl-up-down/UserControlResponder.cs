using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace application_hook_ctrl_up_down
{
    public partial class UserControlResponder : UserControl
    {
        public UserControlResponder()
        {
            InitializeComponent();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if(!(DesignMode || _isHandleInitialized))
            {
                _isHandleInitialized = true;
                var main = (MainForm)Parent;
                main.ControlLeft += onControlLeft;
                main.ControlRight += onControlRight;
                main.NoCommand += onNoCommand;
            }
        }
        private bool _isHandleInitialized = false;

        private void onControlLeft(object sender, EventArgs e)
        {           
            BackColor = Color.LightSalmon;
            BorderStyle = BorderStyle.None;
            if(flowLayoutPanel.Controls.Count != 0)
            {
                var remove = flowLayoutPanel.Controls[flowLayoutPanel.Controls.Count - 1];
                if(remove is Button button)
                {
                    button.Click -= onAnyButtonClick;
                }
                flowLayoutPanel.Controls.RemoveAt(flowLayoutPanel.Controls.Count - 1);
            }
        }
        private void onAnyButtonClick(object sender, EventArgs e)
        {
            ((MainForm)Parent).Text = $"{((Button)sender).Text} Clicked";
        }
        private void onControlRight(object sender, EventArgs e)
        {
            BackColor = Color.LightGreen;
            BorderStyle = BorderStyle.None;
            var button = new Button
                {
                    Text = $"Button {_tstCount++}",
                    Size = new Size(150, 50),
                };
            button.Click += onAnyButtonClick;
            flowLayoutPanel.Controls.Add(button);
        }
        char _tstCount = 'A';

        private void onNoCommand(object sender, EventArgs e)
        {
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
