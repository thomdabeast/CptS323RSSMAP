using System.Windows.Controls;

namespace Button_Crushers_RSS_Client
{
    public abstract class Builder<T>
    {
        protected T thing;

        public virtual T Build(string name)
        {
            return thing;
        }
    }

    public class PlainBuilder : Builder<TreeViewItem>
    {
        public PlainBuilder() { thing = new TreeViewItem(); }
        public PlainBuilder(string name)
        {
            thing = new TreeViewItem { Header = name };
        }

        public override TreeViewItem Build(string name)
        {
            return new TreeViewItem { Header = name, ContextMenu = new ContextMenu() };
        }
    }

    public class CheckBoxBuilder : Builder<TreeViewItem>
    {
        public CheckBoxBuilder() { thing = new TreeViewItem(); }

        public CheckBoxBuilder(TreeViewItem tvi)
        {
            thing = tvi;
        }
        
        public override TreeViewItem Build(string name)
        {
            TreeViewItem tvi = new TreeViewItem();

            CheckBox cb = new CheckBox();
            cb.Content = name;
            cb.Click += InterfaceDataSource.Cb_Checked;

            tvi.Header = cb;

            tvi.ContextMenu = new ContextMenu();

            return tvi;
        }
        
    }
}
