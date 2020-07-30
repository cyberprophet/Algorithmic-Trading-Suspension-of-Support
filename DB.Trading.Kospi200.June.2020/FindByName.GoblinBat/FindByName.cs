using System.Reflection;
using System.Windows.Forms;

namespace ShareInvest.FindByName
{
    public static class FindByNameUtil
    {
        public static T FindByName<T>(this object targetClass, string name) where T : class => targetClass.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(targetClass) as T;
        public static T FindByName<T>(this string name, object targetClass) where T : class => targetClass.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(targetClass) as T;
        public static RadioButton RadioButton(string name, object targetClass) => FindByName<RadioButton>(targetClass, name);
        public static RadioButton Is_RadioButton_Form(this string name, object targetClass) => FindByName<RadioButton>(targetClass, name);
        public static Label Label(string name, object targetClass) => FindByName<Label>(targetClass, name);
        public static Label Is_Label_From(this string name, object targetClass) => FindByName<Label>(targetClass, name);
        public static TextBox TextBox(string name, object targetClass) => FindByName<TextBox>(targetClass, name);
        public static TextBox Is_TextBox_From(this string name, object targetClass) => FindByName<TextBox>(targetClass, name);
        public static NumericUpDown NumericUpDown(string name, object targetClass) => FindByName<NumericUpDown>(targetClass, name);
        public static NumericUpDown Is_NumericUpDown_From(this string name, object targetClass) => FindByName<NumericUpDown>(targetClass, name);
        public static CheckBox CheckBox(this string name, object targetClass) => FindByName<CheckBox>(targetClass, name);
        public static CheckBox Is_CheckBox_From(this string name, object targetClass) => FindByName<CheckBox>(targetClass, name);
        public static GroupBox GroupBox(this string name, object targetClass) => FindByName<GroupBox>(targetClass, name);
        public static GroupBox Is_GroupBox_From(this string name, object targetClass) => FindByName<GroupBox>(targetClass, name);
    }
}