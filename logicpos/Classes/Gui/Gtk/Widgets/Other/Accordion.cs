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

            HeightRequest = 35;
            ExposeEvent += delegate { SetAlignment(0.00F, 0.5F); };
            ChildBox = new VBox();
            ChildBox.ExposeEvent += delegate { if (!Active) ChildBox.Visible = false; };
        }

        public AccordionParentButton(Widget pHeader)
            : base(pHeader)
        {
            HeightRequest = 35;
            //ExposeEvent += delegate { SetAlignment(0.00F, 0.5F); };
            ChildBox = new VBox();
            ChildBox.ExposeEvent += delegate { if (!Active) ChildBox.Visible = false; };
        }
    }

    public class AccordionChildButton : Button
    {
        public Widget Content { get; set; }
        public string ExternalAppFileName { get; set; }

        public AccordionChildButton(String pLabel)
            : base(pLabel)
        {
            HeightRequest = 25;
            ExposeEvent += delegate { SetAlignment(0.0F, 0.5F); };
        }
    }

    public class Accordion : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _nodePrivilegesTokenFormat;

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
            String fontPosBackOfficeParent = GlobalFramework.Settings["fontPosBackOfficeParent"];
            String fontPosBackOfficeChild = GlobalFramework.Settings["fontPosBackOfficeChild"];

            //Parameters
            _accordionDefinition = pAccordionDefinition;
            //Local Vars
            bool isFirstButton = true;
            string currentNodePrivilegesToken;

            VBox vboxOuter = new VBox(false, 2);
            AccordionParentButton accordionParentButton;
            AccordionChildButton accordionChildButton;

            if (_accordionDefinition != null && _accordionDefinition.Count > 0)
            {
                foreach (var parentLevel in _accordionDefinition)
                {
                    if (parentLevel.Value.GroupIcon != null)
                    {
                        HBox hboxParent = new HBox(false, 0);
                        hboxParent.PackStart(parentLevel.Value.GroupIcon, false, false, 3);
                        Label label = new Label(parentLevel.Value.Label);

                        //Pango.FontDescription tmpFont = new Pango.FontDescription();
                        Pango.FontDescription fontDescriptionParent = Pango.FontDescription.FromString(fontPosBackOfficeParent);
                        //tmpFont.Weight = Pango.Weight.Bold;
                        //tmpFont.Size = 2;
                        label.ModifyFont(fontDescriptionParent);
                        label.SetAlignment(0.0f, 0.5f);
                        hboxParent.PackStart(label, true, true, 0);
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
                    if (parentLevel.Value.Childs.Count > 0)
                    {
                        foreach (var childLevel in parentLevel.Value.Childs)
                        {
                            //Init ChildButton
                            accordionChildButton = new AccordionChildButton(childLevel.Value.Label) { Name = childLevel.Key, Content = childLevel.Value.Content };
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
            GlobalFramework.LoggedUser = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(SYS_UserDetail), GlobalFramework.LoggedUser.Oid);
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
    }
}
