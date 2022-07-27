# application-hook-ctrl-up-down
Installing Hotkeys using MessageFilter

The example implements a [IMessageFilter](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.imessagefilter?view=windowsdesktop-6.0) in the MainForm and to
the `WM_KEYDOWN` message for Control-Left and Control-Right. These events are consumed by a `UserControlResponder` that will add and remove buttons based on Control-Right and Control-Left respectively.


***
**Adding and Removing the MessageFilter**

The message filter will be added on the `OnHandleCreated` override and removed in the `Dispose` method of `MainForm`.

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
    }

***
**Handling `PreFilterMessage`**

The MainForm will provide three `event` hooks for `ControlLeft`, `ControlRight`, and `NoCommand`. These will be fired in the `PreFilterMessage` method.


    public partial class MainForm : Form, IMessageFilter
    {
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

***
**Responding to events in the UserControl**

The `MainForm` events will be subscribed to in the `OnHandleCreated` override of `UserControlResponder`.

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
            BackColor = Color.LightGreen;
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
            BackColor = Color.LightBlue;
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
