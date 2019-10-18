using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class AccordionNode
    {
        public string Label { get; set; }
        public Dictionary<string, AccordionNode> Childs { get; set; }
        public Widget NodeButton { get; set; }
        public Widget Content { get; set; }
        public Image GroupIcon { get; set; }
        public string ExternalAppFileName { get; set; }

        //EventHandlers
        public EventHandler Clicked { get; set; }

        public AccordionNode(String pLabel)
        {
            Label = pLabel;
        }
    }

    public class AccordionParentButton : Button
    {        
        public bool Active { get; set; }
        public VBox ChildBox { get; set; }

        public AccordionParentButton(String pLabel)
            : base(pLabel)
        {
            //Redimensionar altura do botão Parent do accordion para 1024
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                HeightRequest = 25;
            }
            else
            {
                HeightRequest = 35;
            }

            ExposeEvent += delegate { SetAlignment(0.00F, 0.5F); };
            ChildBox = new VBox();
            ChildBox.ExposeEvent += delegate { if (!Active) ChildBox.Visible = false; };
        }

        public AccordionParentButton(Widget pHeader)
            : base(pHeader)
        {
            //Redimensionar altura do botão child do accordion para 1024
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                HeightRequest = 25;
            }
            else
            {
                HeightRequest = 35;
            }
            //ExposeEvent += delegate { SetAlignment(0.00F, 0.5F); };
            ChildBox = new VBox();
            ChildBox.ExposeEvent += delegate { if (!Active) ChildBox.Visible = false; };
        }
    }

    public class AccordionChildButton : Button
    {
        public Widget Content { get; set; }
        public string ExternalAppFileName { get; set; }
        public VBox ChildbuttonBox { get; set; }

        public AccordionChildButton(String pLabel)
            : base(pLabel)
        {
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                HeightRequest = 23;
            }
            else
            {
                HeightRequest = 25;
            }
            ExposeEvent += delegate { SetAlignment(0.0F, 0.5F); };
        }

        public AccordionChildButton(Widget pHeader)
            : base(pHeader)
        {
            if (GlobalApp.boScreenSize.Height <= 800)
            {
                HeightRequest = 23;
            }
            else
            {
                HeightRequest = 25;
            }
            ExposeEvent += delegate { SetAlignment(0.0F, 0.5F); };
            ChildbuttonBox = new VBox();
        }
    }

    public class Accordion : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _nodePrivilegesTokenFormat;
        protected Label _label;

        //Declare public Event, to link to accordionChildButton_Clicked
        public event EventHandler Clicked;

        //Public Members
        protected Dictionary<string, AccordionNode> _accordionDefinition;
        internal Dictionary<string, AccordionNode> Nodes
        {
            get { return _accordionDefinition; }
            set { _accordionDefinition = value; }
        }

        protected AccordionParentButton _currentParentButton;
        public AccordionParentButton CurrentParentButton
        {
            get { return _currentParentButton; }
            set { _currentParentButton = value; }
        }

        protected AccordionChildButton _currentChildButton;
        public AccordionChildButton CurrentChildButton
        {
            get { return _currentChildButton; }
            set { _currentChildButton = value; }
        }

        protected AccordionChildButton _currentChildButtonContent;


        public AccordionChildButton CurrentChildButtonContent
        {
            get { return _currentChildButtonContent; }
            set { _currentChildButtonContent = value; }
        }

        public Accordion() { }
        public Accordion(Dictionary<string, AccordionNode> pAccordionDefinition, string pNodePrivilegesTokenFormat)
        {
            _nodePrivilegesTokenFormat = pNodePrivilegesTokenFormat;
            InitObject(pAccordionDefinition, pNodePrivilegesTokenFormat);
        }

        protected void InitObject(Dictionary<string, AccordionNode> pAccordionDefinition, string pNodePrivilegesTokenFormat)
        {
            //get values of backoffice screen to set accordion font text size 
            GlobalApp.boScreenSize = Utils.GetScreenSize();
            _label = new Label();
            
            //Parameters
            _accordionDefinition = pAccordionDefinition;
            //Local Vars
            bool isFirstButton = true;
            string currentNodePrivilegesToken;
            string accordionType = "";

            VBox vboxOuter = new VBox(false, 2);
            AccordionParentButton accordionParentButton;
            AccordionChildButton accordionChildButton;

            if (_accordionDefinition != null && _accordionDefinition.Count > 0)
            {
                foreach (var parentLevel in _accordionDefinition)
                {
                    if (parentLevel.Value.GroupIcon != null)
                    {
                        //Redimensionar Icons dos Botões Parent do accordion para 1024
                        HBox hboxParent = new HBox(false, 0);
                        if (GlobalApp.boScreenSize.Height <= 800)
                        {
                            System.Drawing.Size sizeIcon = new System.Drawing.Size(20, 20);
                            System.Drawing.Image imageIcon;
                            imageIcon = System.Drawing.Image.FromFile(parentLevel.Value.GroupIcon.File.ToString());
                            imageIcon = Utils.ResizeAndCrop(imageIcon, sizeIcon);
                            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(imageIcon);
                            Image gtkimageButton = new Image(pixBuf);
                            hboxParent.PackStart(gtkimageButton, false, false, 3);
                            imageIcon.Dispose();
                            pixBuf.Dispose();
                        }
                        else
                        {
                            hboxParent.PackStart(parentLevel.Value.GroupIcon, false, false, 3);
                        }
                        _label = new Label(parentLevel.Value.Label);
                        //Pango.FontDescription tmpFont = new Pango.FontDescription();          
                        //tmpFont.Weight = Pango.Weight.Bold;
                        //tmpFont.Size = 2;
                        //Redimensionar Tamanho da Fonte dos Botões Parent do accordion para 1024
                        accordionType = "Parent";
                        ChangeFont(FrameworkUtils.StringToColor("61, 61, 61"), accordionType);
                        hboxParent.PackStart(_label, true, true, 0);
                        accordionParentButton = new AccordionParentButton(hboxParent) { Name = parentLevel.Key };
                    }
                    else
                    {
                        accordionParentButton = new AccordionParentButton(parentLevel.Value.Label) { Name = parentLevel.Key };
                        //First Parent Node is Assigned has currentParentButton
                        if (_currentParentButton == null)
                        {
                            _currentParentButton = accordionParentButton;
                        }
                    }

                    accordionParentButton.Active = isFirstButton;
                    if (isFirstButton)
                    {
                        isFirstButton = false;
                    }                    

                    //Add a Button Widget Reference to NodeWidget AccordionDefinition
                    parentLevel.Value.NodeButton = accordionParentButton;
                    //Click Event
                    accordionParentButton.Clicked += accordionParentButton_Clicked;
                    vboxOuter.PackStart(accordionParentButton, false, false, 0);                    
                    //_log.Debug(string.Format("Accordion(): parentLevel.Value.Label [{0}]", parentLevel.Value.Label));
                    if (parentLevel.Value.Childs != null && parentLevel.Value.Childs.Count > 0)
                    {
                        foreach (var childLevel in parentLevel.Value.Childs)
                        {
                            HBox hboxChild = new HBox(false, 0);
                            _label = new Label(childLevel.Value.Label);
                            accordionType = "Child";
                            ChangeFont(FrameworkUtils.StringToColor("61, 61, 61"), accordionType);
                            hboxChild.PackStart(_label, true, true, 0);
                            
                            //Init ChildButton
                            accordionChildButton = new AccordionChildButton(hboxChild) { Name = childLevel.Key, Content = childLevel.Value.Content };
                            //accordionChildButton = new AccordionChildButton(childLevel.Value.Label) { Name = childLevel.Key, Content = childLevel.Value.Content };
                            //Add a Button Widget Reference to NodeWidget AccordionDefinition
                            childLevel.Value.NodeButton = accordionChildButton;
                            //Privileges
                            currentNodePrivilegesToken = string.Format(pNodePrivilegesTokenFormat, childLevel.Key.ToUpper());
                            //_log.Debug(string.Format("currentNodePrivilegesToken: [{0}]", currentNodePrivilegesToken));

                            //First Child Node is Assigned has currentChildButton
                            //if (childLevel.Value.Active)
                            if (_currentChildButton == null)
                            {
                                _currentChildButton = accordionChildButton;
                                //Assign Current Active Button with content
                                _currentChildButtonContent = accordionChildButton;
                            }

                            accordionParentButton.ChildBox.PackStart(accordionChildButton, false, false, 2);

                            //If have (Content | Events | ExternalApp) & Privileges or the Button is Enabled, Else is Disabled
                            accordionChildButton.Sensitive = (FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken) && (childLevel.Value.Content != null || childLevel.Value.Clicked != null || childLevel.Value.ExternalAppFileName != null));

                            //EventHandler, Redirected to public Clicked, this way we have ouside Access
                            accordionChildButton.Clicked += accordionChildButton_Clicked;
                            //ExternalAppFileName
                            if (childLevel.Value.ExternalAppFileName != null) accordionChildButton.ExternalAppFileName = childLevel.Value.ExternalAppFileName;

                            //Process AccordionDefinition Clicked Events
                            if (childLevel.Value.Clicked != null)
                            {
                                accordionChildButton.Clicked += childLevel.Value.Clicked;
                            }
                        }
                        vboxOuter.PackStart(accordionParentButton.ChildBox, false, false, 0);
                    }
                }
            }
            PackStart(vboxOuter);
        }

        public void UpdateMenuPrivileges()
        {
            string currentNodePrivilegesToken;

            //Required to Reload Object before Get New Permissions
            GlobalFramework.LoggedUser = (sys_userdetail)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(sys_userdetail), GlobalFramework.LoggedUser.Oid);
            //Update Session Privileges
            GlobalFramework.LoggedUserPermissions = FrameworkUtils.GetUserPermissions(GlobalFramework.LoggedUser);

            //Update Backoffice Menu
            if (_accordionDefinition != null && _accordionDefinition.Count > 0)
            {
                foreach (var parentLevel in _accordionDefinition)
                {
                    if (parentLevel.Value.Childs.Count > 0)
                    {
                        foreach (var childLevel in parentLevel.Value.Childs)
                        {
                            currentNodePrivilegesToken = string.Format(_nodePrivilegesTokenFormat, childLevel.Key.ToUpper());
                            //_log.Debug(string.Format("[{0}]=[{1}] [{2}]=[{3}]", childLevel.Value.NodeButton.Sensitive, childLevel.Value.NodeButton.Name, currentNodePrivilegesToken, FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken)));
                            //If have (Content | Events | ExternalApp) & Privileges or the Button is Enabled, Else is Disabled
                            if (FrameworkUtils.HasPermissionTo(currentNodePrivilegesToken) && (childLevel.Value.Content != null || childLevel.Value.Clicked != null || childLevel.Value.ExternalAppFileName != null))
                            {
                                childLevel.Value.NodeButton.Sensitive = true;
                            }
                            else
                            {
                                childLevel.Value.NodeButton.Sensitive = false;
                            }
                        }
                    }
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events
        private void accordionParentButton_Clicked(object sender, EventArgs e)
        {
            AccordionParentButton clickedButton = (AccordionParentButton)sender;

            foreach (var item in _accordionDefinition)
            {
                _currentParentButton = (AccordionParentButton)item.Value.NodeButton;

                if (!_currentParentButton.Equals(clickedButton))
                {
                    _currentParentButton.Active = false;
                    _currentParentButton.ChildBox.Visible = false;
                }
                else
                {
                    _currentParentButton.Active = true;
                    _currentParentButton.ChildBox.Visible = true;
                }
            }

            if (Clicked != null)
            {
                Clicked(sender, e);
            }
        }

        //Redirect to public Clicked Event, this way we can have access to ChildButtons Click Events from the outside 
        private void accordionChildButton_Clicked(object sender, EventArgs e)
        {
            _currentChildButton = (AccordionChildButton)sender;
            if (Clicked != null)
            {
                Clicked(sender, e);
            }
        }
       
        public void ChangeFont(System.Drawing.Color pColorFont, string accordionType)
        {          
            //color
            System.Drawing.Color colNormal = pColorFont;
            System.Drawing.Color colPrelight = Utils.Lighten(colNormal);
            System.Drawing.Color colActive = Utils.Lighten(colPrelight);
            System.Drawing.Color colInsensitive = Utils.Darken(colNormal);
            System.Drawing.Color colSelected = System.Drawing.Color.FromArgb(125, 0, 0);

            String _fontPosBackOfficeParent = GlobalFramework.Settings["fontPosBackOfficeParent"];
            String _fontPosBackOfficeChild = GlobalFramework.Settings["fontPosBackOfficeChild"];
            String _fontPosBackOfficeParentLowRes = GlobalFramework.Settings["fontPosBackOfficeParentLowRes"];
            String _fontPosBackOfficeChildLowRes = GlobalFramework.Settings["fontPosBackOfficeChildLowRes"];

            Pango.FontDescription fontPosBackOfficeparentLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeParentLowRes);
            Pango.FontDescription fontPosBackOfficeParent = Pango.FontDescription.FromString(_fontPosBackOfficeParent);
            Pango.FontDescription fontPosBackOfficeChildLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeChildLowRes);
            Pango.FontDescription fontPosBackOfficeChild = Pango.FontDescription.FromString(_fontPosBackOfficeChild);

            if(accordionType == "Parent")
            {
                if (GlobalApp.boScreenSize.Height <= 800) _label.ModifyFont(fontPosBackOfficeparentLowRes);
                else _label.ModifyFont(fontPosBackOfficeParent);
            }
            else
            {
                if (GlobalApp.boScreenSize.Height <= 800) _label.ModifyFont(fontPosBackOfficeChildLowRes);
                else _label.ModifyFont(fontPosBackOfficeChild);
            }
            _label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colNormal));
            _label.ModifyFg(StateType.Prelight, Utils.ColorToGdkColor(colPrelight));
            _label.ModifyFg(StateType.Active, Utils.ColorToGdkColor(colActive));
            _label.ModifyFg(StateType.Insensitive, Utils.ColorToGdkColor(colInsensitive));
            _label.ModifyFg(StateType.Selected, Utils.ColorToGdkColor(colSelected));
            
            _label.SetAlignment(0.0f, 0.5f);
        }
    }
}
