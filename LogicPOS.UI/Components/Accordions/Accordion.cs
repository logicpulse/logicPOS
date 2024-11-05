using Gtk;
using logicpos;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Extensions;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Accordions
{
    public class Accordion : Box
    {
        private readonly string _permissionTokenFormat;
        protected Label _label;
        public event EventHandler Clicked;
        internal Dictionary<string, AccordionNode> Nodes;
        public AccordionParentButton CurrentParent { get; set; }
        public AccordionChildButton CurrentChild { get; set; }
        public AccordionChildButton CurrentPageChildButton { get; set; }

        public Accordion() { }
        public Accordion(Dictionary<string, AccordionNode> nodes,
                         string permissionTokenFormat)
        {
            _permissionTokenFormat = permissionTokenFormat;
            Initialize(nodes, permissionTokenFormat);
        }

        protected void Initialize(Dictionary<string, AccordionNode> nodes,
                                  string nodePrivilegesTokenFormat)
        {
            LogicPOSAppContext.BackOfficeScreenSize = Utils.GetScreenSize();
            _label = new Label();
            Nodes = nodes;

            bool isFirstButton = true;
            string currentNodePrivilegesToken;
            string accordionType = "";

            VBox vboxOuter = new VBox(false, 2);
            AccordionParentButton accordionParentButton;
            AccordionChildButton accordionChildButton;

            if (Nodes != null && Nodes.Count > 0)
            {
                foreach (var node in Nodes)
                {
                    if (node.Value.GroupIcon != null)
                    {
                        HBox button = new HBox(false, 0);

                        if (LogicPOSAppContext.BackOfficeScreenSize.Height <= 800)
                        {
                            System.Drawing.Size sizeIcon = new System.Drawing.Size(20, 20);
                            System.Drawing.Image imageIcon;
                            imageIcon = System.Drawing.Image.FromFile(node.Value.GroupIcon.File.ToString());
                            imageIcon = Utils.ResizeAndCrop(imageIcon, sizeIcon);
                            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(imageIcon);
                            Image gtkimageButton = new Image(pixBuf);
                            button.PackStart(gtkimageButton, false, false, 3);
                            imageIcon.Dispose();
                            pixBuf.Dispose();
                        }
                        else
                        {
                            button.PackStart(node.Value.GroupIcon, false, false, 3);
                        }
                        _label = new Label(node.Value.Label);

                        accordionType = "Parent";
                        ChangeFont("61, 61, 61".StringToColor(), accordionType);
                        button.PackStart(_label, true, true, 0);
                        accordionParentButton = new AccordionParentButton(button) { Name = node.Key };
                    }
                    else
                    {
                        accordionParentButton = new AccordionParentButton(node.Value.Label) { Name = node.Key };

                        if (CurrentParent == null)
                        {
                            CurrentParent = accordionParentButton;
                        }
                    }

                    accordionParentButton.Active = isFirstButton;

                    if (isFirstButton)
                    {
                        isFirstButton = false;
                    }

                    node.Value.Button = accordionParentButton;

                    accordionParentButton.Clicked += ParentButton_Clicked;
                    vboxOuter.PackStart(accordionParentButton, false, false, 0);

                    if (node.Value.Children != null && node.Value.Children.Count > 0)
                    {
                        foreach (var childLevel in node.Value.Children)
                        {
                            HBox hboxChild = new HBox(false, 0);
                            _label = new Label(childLevel.Value.Label);
                            accordionType = "Child";
                            ChangeFont("61, 61, 61".StringToColor(), accordionType);
                            hboxChild.PackStart(_label, true, true, 0);

                            accordionChildButton = new AccordionChildButton(hboxChild) { Name = childLevel.Key, Page = childLevel.Value.Content };

                            childLevel.Value.Button = accordionChildButton;


                            currentNodePrivilegesToken = string.Format(nodePrivilegesTokenFormat, childLevel.Key.ToUpper());

                            if (CurrentChild == null)
                            {
                                CurrentChild = accordionChildButton;
                                CurrentPageChildButton = accordionChildButton;
                            }

                            accordionParentButton.PanelButtons.PackStart(accordionChildButton, false, false, 2);

                            accordionChildButton.Sensitive = GeneralSettings.LoggedUserHasPermissionTo(currentNodePrivilegesToken) && (childLevel.Value.Content != null || childLevel.Value.Clicked != null || childLevel.Value.ExternalAppFileName != null) && childLevel.Value.Sensitive;

                            accordionChildButton.Clicked += ChildButton_Clicked;

                            if (childLevel.Value.ExternalAppFileName != null)
                            {
                                accordionChildButton.ExternalApplication = childLevel.Value.ExternalAppFileName;
                            }

                            if (childLevel.Value.Clicked != null)
                            {
                                accordionChildButton.Clicked += childLevel.Value.Clicked;
                            }
                        }
                        vboxOuter.PackStart(accordionParentButton.PanelButtons, false, false, 0);
                    }
                }
            }
            PackStart(vboxOuter);
        }

        public void UpdateMenuPrivileges()
        {
            string token;

            if (Nodes != null && Nodes.Count > 0)
            {
                foreach (var parentLevel in Nodes)
                {
                    if (parentLevel.Value.Children.Count > 0)
                    {
                        foreach (var childLevel in parentLevel.Value.Children)
                        {
                            token = string.Format(_permissionTokenFormat, childLevel.Key.ToUpper());

                            if (AuthenticationService.UserHasPermission(token) && (childLevel.Value.Content != null || childLevel.Value.Clicked != null || childLevel.Value.ExternalAppFileName != null))
                            {
                                childLevel.Value.Button.Sensitive = true;
                            }
                            else
                            {
                                childLevel.Value.Button.Sensitive = false;
                            }
                        }
                    }
                }
            }
        }


        private void ParentButton_Clicked(object sender, EventArgs e)
        {
            AccordionParentButton button = (AccordionParentButton)sender;

            foreach (var item in Nodes)
            {
                CurrentParent = (AccordionParentButton)item.Value.Button;

                if (CurrentParent != button)
                {
                    CurrentParent.Active = false;
                    CurrentParent.PanelButtons.Visible = false;
                }
                else
                {
                    CurrentParent.Active = true;
                    CurrentParent.PanelButtons.Visible = true;
                }
            }

            Clicked?.Invoke(sender, e);
        }

        private void ChildButton_Clicked(object sender, EventArgs e)
        {
            CurrentChild = (AccordionChildButton)sender;
            Clicked?.Invoke(sender, e);
        }

        public void ChangeFont(System.Drawing.Color pColorFont, string accordionType)
        {
            //color
            System.Drawing.Color colNormal = pColorFont;
            System.Drawing.Color colPrelight = colNormal.Lighten();
            System.Drawing.Color colActive = colPrelight.Lighten();
            System.Drawing.Color colInsensitive = colNormal.Darken();
            System.Drawing.Color colSelected = System.Drawing.Color.FromArgb(125, 0, 0);

            string _fontPosBackOfficeParent = AppSettings.Instance.fontPosBackOfficeParent;
            string _fontPosBackOfficeChild = AppSettings.Instance.fontPosBackOfficeChild;
            string _fontPosBackOfficeParentLowRes = AppSettings.Instance.fontPosBackOfficeParentLowRes;
            string _fontPosBackOfficeChildLowRes = AppSettings.Instance.fontPosBackOfficeChildLowRes;

            Pango.FontDescription fontPosBackOfficeparentLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeParentLowRes);
            Pango.FontDescription fontPosBackOfficeParent = Pango.FontDescription.FromString(_fontPosBackOfficeParent);
            Pango.FontDescription fontPosBackOfficeChildLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeChildLowRes);
            Pango.FontDescription fontPosBackOfficeChild = Pango.FontDescription.FromString(_fontPosBackOfficeChild);

            if (accordionType == "Parent")
            {
                if (LogicPOSAppContext.BackOfficeScreenSize.Height <= 800) _label.ModifyFont(fontPosBackOfficeparentLowRes);
                else _label.ModifyFont(fontPosBackOfficeParent);
            }
            else
            {
                if (LogicPOSAppContext.BackOfficeScreenSize.Height <= 800) _label.ModifyFont(fontPosBackOfficeChildLowRes);
                else _label.ModifyFont(fontPosBackOfficeChild);
            }
            _label.ModifyFg(StateType.Normal, colNormal.ToGdkColor());
            _label.ModifyFg(StateType.Prelight, colPrelight.ToGdkColor());
            _label.ModifyFg(StateType.Active, colActive.ToGdkColor());
            _label.ModifyFg(StateType.Insensitive, colInsensitive.ToGdkColor());
            _label.ModifyFg(StateType.Selected, colSelected.ToGdkColor());

            _label.SetAlignment(0.0f, 0.5f);
        }
    }
}
