using System.Collections.Generic;
using System.Collections.ObjectModel;
using PdfExtractor.Models;

namespace Viewer.Wpf
{
    internal interface ITreeNode
    {
        int ID { get; }

        AggregatedMoney Money { get; }
        
        string Title { get; }

        ICollection<ITreeNode> SubCollection { get; }
    }
    
    internal class InnerNode : ITreeNode
    {
        public int ID { get; set; }

        public AggregatedMoney Money
        {
            get
            {
                var money = new AggregatedMoney();
                foreach (var node in SubCollection)
                {
                    money.Add(node.Money);
                }

                return money;
            }
        }
        
        public string Title { get; set; } = string.Empty;

        public ICollection<ITreeNode> SubCollection { get; } = new ObservableCollection<ITreeNode>();
    }

    internal class EndNode : ITreeNode
    {
        public int ID { get; }
        
        public AggregatedMoney Money { get; set; }
        
        public string Title { get; set; }
        
        public ICollection<ITreeNode> SubCollection { get; }
    }
}
