using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Viewer
{
    internal class TreeNode
    {
        public int ID { get; set; }

        public int Money { get; set; }
        
        public string Title { get; set; } = string.Empty;

        public ICollection<TreeNode> SubCollection { get; set; } = new ObservableCollection<TreeNode>();
    }
}
