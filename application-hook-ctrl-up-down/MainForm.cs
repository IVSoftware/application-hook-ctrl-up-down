using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace application_hook_ctrl_up_down
{
    public partial class MainForm : Form, IMessageFilter
    {
        public MainForm() => InitializeComponent();
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if(!(DesignMode ) || _isHandleInitialized)
            {
                _isHandleInitialized = true; ;
                Application.AddMessageFilter(this);
            }
        }
        bool _isHandleInitialized = false;

        // In MainForm.Designer.cs
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                Application.RemoveMessageFilter(this);
            }
            base.Dispose(disposing);
        }

        const int WM_KEYDOWN = 0x100;
        public bool PreFilterMessage(ref Message m)
		{
			if (Form.ActiveForm == this)
			{
				switch (m.Msg)
				{
					case WM_KEYDOWN:
						var key = (Keys)m.WParam | ModifierKeys;
                        switch (key)
                        {
                            case Keys.Control | Keys.Left:
                                ControlLeft?.Invoke(this, EventArgs.Empty);
                                Text = "Control.Left";
                                break;
                            case Keys.Control | Keys.Right:
                                ControlRight?.Invoke(this, EventArgs.Empty);
                                Text = "Control.Right";
                                break;
                            default: 
                                // Don't event if it's "just" the Control key
                                if(ModifierKeys == Keys.None)
                                {
                                    Text = "Main Form";
                                    NoCommand?.Invoke(this, EventArgs.Empty);
                                }
                                break;
                        }
                        break;
				}
			}
			return false;
        }

        public event EventHandler ControlLeft;
        public event EventHandler ControlRight;
        public event EventHandler NoCommand;
    }
}
