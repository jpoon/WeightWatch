using System.Collections.Generic;

namespace WeightWatch.Classes
{
    public class LongListSelectorGroup<T> : IEnumerable<T>
    {
        public LongListSelectorGroup(string date, IEnumerable<T> items)
        {
            this.GroupHeader = date;
            this.GroupItem = date;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            LongListSelectorGroup<T> that = obj as LongListSelectorGroup<T>;

            return (that != null) && (this.GroupHeader.Equals(that.GroupHeader));
        }

        public string GroupHeader
        {
            get;
            set;
        }

        public string GroupItem
        {
            get;
            set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}
