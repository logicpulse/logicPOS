using System;
using Gtk;

namespace logicpos
{
    class Theme
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ParseTheme(bool useTheme = true, bool debug = false)
        {
            if (debug)
            {
                Console.WriteLine("Gtk.Rc.ModuleDir:" + Gtk.Rc.ModuleDir);
            }

            if (useTheme) Gtk.Rc.ParseString(@"

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

#Default Colors
#gtk_color_scheme = 'fg_color:#000000\
#  bg_color:#E8E8E8\
#  base_color:#FFFFFF\
#  text_color:#000000\
#  selected_bg_color:#1F87D2\
#  selected_fg_color:#FFFFFF\
#'

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

#Global Font
style 'pFont'
{
  font_name = 'Corbel 12'
}
#widget_class '*' style 'pFont'
gtk-pFont-pName = 'Corbel 12'
gtk-pFont-pName = 'Tahoma 16'

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

#style 'widgets' 
#{ 
#  #GtkButton::inner-border           = { 10, 10, 10, 10 }
#  #GtkButton::default_outside_border = {5, 5, 5, 5}
#  #GtkButton::child_displacement_x   = 1
#  #GtkButton::child_displacement_y   = 1
#  #GtkButton::default_spacing        = 5
#  #GtkButton::focus-pPadding          = 5
#  #GtkButton::default_border         = {5, 5, 5, 5}
#
#  engine 'nodoka'
#  {
#    roundness = 5
#  }
#}

style 'buttons' 
{ 
  font_name = 'Tahoma 10'

  engine 'nodoka'
  {
    roundness = 5
  }
}

style 'labels' = 'widgets' 
{ 
  font_name = 'Sans Bold 14'
}

style 'TreeView'
{
  font_name = '12'
  #fg[NORMAL]        = '#FF0000'
  #bg[NORMAL]			   = '#0000FF'
  #base[NORMAL]			 = '#00FF00'
  #text[NORMAL]			 = '#0000FF'
}

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

style 'TouchButton'
{
  color['background'] = '#a5a5a5' #'#b9b9b9'

  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

style 'TouchButton_Grey'
{
  color['background'] = '#aaaaaa'

  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding         = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

style 'TouchButton_DarkGrey'
{
  color['background'] = '#485460'

  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

style 'TouchButton_Red'
{
  color['background'] = '#c25e4f'

  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

style 'TouchButton_Green'
{
  color['background'] = '#8ba844'

  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding         = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

style 'TouchButton_DialogActionArea'
{
  color['background'] = '#4477a4'
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {1,1,1,1}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding         = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= TRUE
		focus_inner		= FALSE
  }
}

style 'TouchButtonImageFamily'
{ 
  color['background'] = '#8ab53f' 
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {1,1,1,1}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= TRUE
		focus_inner		= FALSE
  }
}

style 'TouchButtonImageSubFamily'
{ 
  color['background'] = '#547b9f'
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {1,1,1,1}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= TRUE
		focus_inner		= FALSE
  }
}

style 'TouchButtonImageArticle'
{ 
  color['background'] = '#e1e1e1' 
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {1,1,1,1}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= TRUE
		focus_inner		= FALSE
  }
}

style 'TouchButtonImageFavorites'
{ 
  color['background'] = '#be66cc' 
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = lighter (@background)
  bg[SELECTED]        = '#323232' # shade (0.25, @background)
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = shade (0.9, @background)

  GtkButton::inner-border           = {1,1,1,1}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= TRUE
		focus_inner		= FALSE
  }
}

style 'TouchButtonImageScrollers'
{ 
  color['background'] = '#3d3d3d' 
  bg[NORMAL]          = @background
  bg[PRELIGHT]        = @background
  bg[SELECTED]        = @background
  bg[INSENSITIVE]     = @background
  bg[ACTIVE]          = @background

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
		focus_fill		= FALSE
		focus_inner		= TRUE
  }
}

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

style 'AccordionParentButton'
{ 
  font_name = 'Tahoma 14'

  color['background'] = '#a3c749'

  bg[NORMAL]         = @background
  bg[PRELIGHT]       = lighter (@background)
  bg[SELECTED]       = shade (0.25, @background)
  bg[INSENSITIVE]    = @background
  bg[ACTIVE]         = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
  }
}

style 'AccordionChildButton'
{ 
  font_name = 'Tahoma 11'

  color['background'] = '#c3d990'

  bg[NORMAL]         = @background
  bg[PRELIGHT]       = lighter (@background)
  bg[SELECTED]       = shade (0.25, @background)
  bg[INSENSITIVE]    = @background
  bg[ACTIVE]         = shade (0.9, @background)

  GtkButton::inner-border           = {0,0,0,0}
  GtkButton::default_spacing        = 0
  GtkButton::focus-pPadding          = 0

  engine 'nodoka'
  {
    roundness = 5
  }
}

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
#style 'KeyboardPadKey' = ' buttons' 
#{
#  color['blackwhite'] = mix (0.5, '#000000', '#FFFFFF')
#  color['darker'] = shade (0.5, @blackwhite) 
#  color['multiple'] = shade (1.4, mix (0.1, '#369', { 0, 1.0, 0 }))
#  color['keyboardkey'] = '#545454'
#
#  #GtkButton::default-outside-border  = {5, 5, 5, 5}
#  #GtkButton::default-border = 5
#
#  bg[NORMAL]         = @keyboardkey
#  bg[PRELIGHT]       = lighter (@keyboardkey)
#  #bg[SELECTED]      = @selected_bg_color
#  bg[INSENSITIVE]    = @keyboardkey
#  bg[ACTIVE]         = shade (0.9, @keyboardkey)
#
#  #fg[NORMAL]        = @fg_color
#  #fg[PRELIGHT]      = @fg_color
#  #fg[SELECTED]      = @selected_fg_color
#  #fg[INSENSITIVE]   = darker (@bg_color)
#  #fg[ACTIVE]        = @fg_color
#
#  xthickness = 2
#	 ythickness = 2	
#
#  engine 'nodoka'
#  {
#    roundness = 5
#  }
#}

##:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

#widget_class '*' style 'default'
#class 'Gtk*Button' style 'buttons'
#widget_class '*KeyboardPadKey' style 'KeyboardKey'
widget_class '*KeyboardPadKey' style 'TouchButton_DarkGrey'

#POS
class '*TouchButton*' style 'TouchButton'

widget_class '*TouchButton*' style 'TouchButton'
#widget_class '*TouchButtonText*' style 'TouchButton'

#Articles
widget '*.buttonFamilyId*' style 'TouchButtonImageFamily'
widget '*.buttonSubFamilyId*' style 'TouchButtonImageSubFamily'
widget '*.buttonArticleId*' style 'TouchButtonImageArticle'
widget '*.buttonFavorites*' style 'TouchButtonImageFavorites'

#Places/Orders/Tables/Users
widget '*.buttonPlaceId*' style 'TouchButtonImageFamily'
widget '*.buttonOrderId*' style 'TouchButtonImageArticle'
widget '*.buttonTableId*' style 'TouchButton_Grey'
widget '*.buttonUserId*' style 'TouchButton_Grey'

#Color TouchButtons
widget '*.touchButton*_Red' style 'TouchButton_Red'
widget '*.touchButton*_DarkGrey' style 'TouchButton_DarkGrey'
widget '*.touchButton*_Green' style 'TouchButton_Green'

#Dialogs
widget '*.touchButton*_DialogActionArea' style 'TouchButton_DialogActionArea'

#POS Scrollers
widget '*.buttonPosScrollers*' style 'TouchButtonImageScrollers'

#Accordion
widget_class '*AccordionParentButton*' style 'AccordionParentButton'
widget_class '*AccordionChildButton*' style 'AccordionChildButton'

#TreeView
#widget_class '*GenericTreeView*.*' style 'KeyboardPadKey'
#widget_class '*TreeView*' style 'TreeView'
      ");
        }
    }
}