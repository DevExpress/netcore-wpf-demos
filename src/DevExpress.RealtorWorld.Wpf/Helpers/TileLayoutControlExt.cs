using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.LayoutControl;

namespace DevExpress.RealtorWorld.Xpf.Helpers {
    public interface ITileLayoutControlExt : ITileLayoutControl {
        void SetTileIsSelected(ITileExt tile, bool isSelected);
    }
    public interface ITileExt {
        bool IsSelected { get; set; }
    }
    public class TileLayoutControlExt : TileLayoutControl, ITileLayoutControlExt {
        public static readonly DependencyProperty SelectedTileProperty =
            DependencyProperty.Register("SelectedTile", typeof(ITileExt), typeof(TileLayoutControlExt), new PropertyMetadata(null,
                (d, e) => ((TileLayoutControlExt)d).OnSelectedTileChanged(e)));
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TileLayoutControlExt), new PropertyMetadata(null,
                (d, e) => ((TileLayoutControlExt)d).OnSelectedItemChanged(e)));

        public object SelectedItem { get { return GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
        public ITileExt SelectedTile { get { return (ITileExt)GetValue(SelectedTileProperty); } set { SetValue(SelectedTileProperty, value); } }

        public ITileExt ItemToTile(object item) {
            return IndexToTile(ItemToIndex(item));
        }
        public object TileToItem(ITileExt tile) {
            return IndexToItem(TileToIndex((ITile)tile));
        }

        void ITileLayoutControlExt.SetTileIsSelected(ITileExt tile, bool isSelected) {
            if(isSelected) {
                SelectedTile = tile;
            } else {
                if(!object.Equals(SelectedTile, tile))
                    SelectedTile = null;
            }
        }
        void OnSelectedTileChanged(DependencyPropertyChangedEventArgs e) {
            ITileExt oldValue = (ITileExt)e.OldValue;
            ITileExt newValue = (ITileExt)e.NewValue;
            SelectedItem = TileToItem(newValue);
            if(oldValue != null)
                oldValue.IsSelected = false;
            if(newValue != null)
                newValue.IsSelected = true;
        }
        void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e) {
            SelectedTile = ItemToTile(e.NewValue);
        }

        int ItemToIndex(object item) {
            var x = item == null ? null : ItemsSource.OfType<object>().Select((o, i) => new { obj = o, index = i }).FirstOrDefault(p => object.Equals(p.obj, item));
            return x == null ? -1 : x.index;
        }
        ITileExt IndexToTile(int index) {
            return index < 0 ? null : Children.OfType<UIElement>().Where(c => !(c is ScrollBar)).ElementAt(index) as ITileExt;
        }
        int TileToIndex(ITile tile) {
            var x = tile == null ? null : Children.OfType<UIElement>().Where(c => !(c is ScrollBar)).Select((o, i) => new { obj = o, index = i }).FirstOrDefault(p => object.Equals(p.obj, tile));
            return x == null ? -1 : x.index;
        }
        object IndexToItem(int index) {
            return index < 0 ? null : ItemsSource.OfType<object>().ElementAt(index);
        }
    }
    public class TileExt : Tile, ITileExt {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TileExt), new PropertyMetadata(false,
                (d, e) => ((TileExt)d).OnIsSelectedChanged(e)));
        public static readonly DependencyProperty SelectOnClickProperty =
            DependencyProperty.Register("SelectOnClick", typeof(bool), typeof(TileExt), new PropertyMetadata(true));

        public ITileLayoutControlExt LayoutControl { get { return Parent as ITileLayoutControlExt; } }
        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
        public bool SelectOnClick { get { return (bool)GetValue(SelectOnClickProperty); } set { SetValue(SelectOnClickProperty, value); } }

        protected override void OnClick() {
            if(SelectOnClick)
                IsSelected = true;
            base.OnClick();
        }
        void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
            bool newValue = (bool)e.NewValue;
            if(LayoutControl != null)
                LayoutControl.SetTileIsSelected(this, newValue);
        }
    }
}
