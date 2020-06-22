
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCustomControlLibrary1.Element;

namespace WpfCustomControlLibrary1
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary1"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary1;assembly=WpfCustomControlLibrary1"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    
        public class MetroScrollViewer : ScrollViewer
        {
            public static readonly DependencyProperty FloatProperty = ElementBase.Property<MetroScrollViewer, bool>(nameof(FloatProperty));
            public static readonly DependencyProperty AutoLimitMouseProperty = ElementBase.Property<MetroScrollViewer, bool>(nameof(AutoLimitMouseProperty));
            public static readonly DependencyProperty VerticalMarginProperty = ElementBase.Property<MetroScrollViewer, Thickness>(nameof(VerticalMarginProperty));
            public static readonly DependencyProperty HorizontalMarginProperty = ElementBase.Property<MetroScrollViewer, Thickness>(nameof(HorizontalMarginProperty));

            public bool Float { get { return (bool)GetValue(FloatProperty); } set { SetValue(FloatProperty, value); } }
            public bool AutoLimitMouse { get { return (bool)GetValue(AutoLimitMouseProperty); } set { SetValue(AutoLimitMouseProperty, value); } }
            public Thickness VerticalMargin { get { return (Thickness)GetValue(VerticalMarginProperty); } set { SetValue(VerticalMarginProperty, value); } }
            public Thickness HorizontalMargin { get { return (Thickness)GetValue(HorizontalMarginProperty); } set { SetValue(HorizontalMarginProperty, value); } }

            public MetroScrollViewer()
            {
                Utility.Refresh(this);
            }

            static MetroScrollViewer()
            {
                ElementBase.DefaultStyle<MetroScrollViewer>(DefaultStyleKeyProperty);
            }
        }
     
}
