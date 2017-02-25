/*
 * Copyright © 2017 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using EDDiscovery;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ExtendedControls
{
    /// <summary>
    /// A <see cref="HistoryEntry"/>-backed <see cref="DataGridViewColumn"/> that will display the
    /// appropriate icon(s) for an event.
    /// </summary>
    public class EventDGVColumn : DataGridViewColumn
    {
        #region DataGridViewEventColumn

        /// <summary>
        /// Gets or sets the template used to create new cells. Required for designer support.
        /// </summary>
        /// <value>A <see cref="DataGridViewCell"/> that all other cells in the column are modeled after. The default is <c>null</c>.</value>
        /// <exception cref="InvalidCastException">raised when <paramref name="CellTemplate"/> does not inherit from <see cref="EventDisplayCell"/>.</exception>
        [Browsable(false)]
        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(EventDisplayCell)))
                    throw new InvalidCastException("value is not an EventDisplayCell");
                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// There is no user-editing of the contents of this column.
        /// </summary>
        [Browsable(false)]
        public override bool ReadOnly { get { return true; } }

        /// <summary>
        /// Whether to show the circle depicting the color displayed on the map for an FSD event.
        /// </summary>
        [Browsable(true)]
        [Description("Whether to show the circle depicting the color displayed on the map for an FSD event.")]
        public bool ShowMapColourForFSD { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDGVColumn"/> class to the default state.
        /// </summary>
        public EventDGVColumn() : base(new EventDisplayCell()) { }

        /// <summary>
        /// Creates an exact copy of this <see cref="EventDGVColumn"/>. Required for designer support.
        /// </summary>
        /// <returns>An <see cref="object"/> that represents the cloned <see cref="EventDGVColumn"/>.</returns>
        public override object Clone()
        {
            var clone = base.Clone() as EventDGVColumn;
            clone.ShowMapColourForFSD = ShowMapColourForFSD;
            return clone;
        }

        #endregion // DataGridViewEventColumn

        #region EventDisplayCell

        /// <summary>
        /// Displays the imagery associated with the assigned <see cref="HistoryEntry"/> event.
        /// </summary>
        private class EventDisplayCell : DataGridViewTextBoxCell
        {
            /// <summary>
            /// Gets the default value for a cell in the row for new records.
            /// </summary>
            public override object DefaultNewRowValue { get { return null; } }

            /// <summary>
            /// Gets the data type of the values in the cell.
            /// </summary>
            public override Type ValueType { get { return typeof(HistoryEntry); } }

            /// <summary>
            /// Whether to show the circle depicting the colour displayed on the map for an FSD event.
            /// Inherited from <see cref="EventDGVColumn.ShowMapColourForFSD"/>.
            /// </summary>
            public bool ShowMapColourForFSD
            {
                get
                {
                    return (OwningColumn as EventDGVColumn).ShowMapColourForFSD;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="EventDisplayCell"/> class.
            /// </summary>
            public EventDisplayCell() : base() { }

            /// <summary>
            /// Paints the current <see cref="EventDisplayCell"/>.
            /// </summary>
            /// <param name="graphics">The <see cref="Graphics"/> used to paint the <see cref="EventDisplayCell"/>.</param>
            /// <param name="clipBounds">A <see cref="Rectangle"/> that represents the area of the <see cref="DataGridView"/> that needs to be repainted.</param>
            /// <param name="cellBounds">A <see cref="Rectangle"/> that contains the bounds of the <see cref="EventDisplayCell"/> that is being painted.</param>
            /// <param name="rowIndex">The row index of the cell that is being painted.</param>
            /// <param name="cellState">A bitwise combination of <see cref="DataGridViewElementStates"/> values that specifies the state of the cell.</param>
            /// <param name="value">The data of the <see cref="EventDisplayCell"/> that is being painted.</param>
            /// <param name="formattedValue">The formatted data of the <see cref="EventDisplayCell"/> that is being painted.</param>
            /// <param name="errorText">An error message that is associated with the cell.</param>
            /// <param name="cellStyle">A <see cref="DataGridViewCellStyle"/> that contains formatting and style information about the cell.</param>
            /// <param name="advancedBorderStyle">A <see cref="DataGridViewAdvancedBorderStyle"/> that contains border styles for the cell that is being painted.</param>
            /// <param name="paintParts">A bitwise combination of the <see cref="DataGridViewPaintParts"/> values that specifies which parts of the cell need to be painted.</param>
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);

                Rectangle padbounds = new Rectangle(
                    cellBounds.Left + cellStyle.Padding.Left, cellBounds.Top + cellStyle.Padding.Top,
                    cellBounds.Width - cellStyle.Padding.Horizontal, cellBounds.Height - cellStyle.Padding.Vertical);
                HistoryEntry he = (HistoryEntry)value;

                if (he == null || padbounds.Height < 4 || padbounds.Width < 4
                    || !(paintParts.HasFlag(DataGridViewPaintParts.ContentForeground) || paintParts.HasFlag(DataGridViewPaintParts.All)))
                {
                    return;
                }
                    
                const int padding = 4;
                int numicons = 1;
                int size = 24;

                if (ShowMapColourForFSD && he.IsFSDJump)
                    numicons++;
                if (he.StartMarker || he.StopMarker)
                    numicons++;

                if (size * numicons > (padbounds.Width - 2))
                    size = (padbounds.Width - 2) / numicons;
                if (size > padbounds.Height)
                    size = padbounds.Height;

                int imgleft = padbounds.X;
                int imgtop = padbounds.Y;
                int imgswidth = size * numicons + padding * (numicons - 1);

                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopCenter:
                        imgleft += (padbounds.Width - imgswidth) / 2;
                        break;
                    case DataGridViewContentAlignment.TopLeft:
                        break;
                    case DataGridViewContentAlignment.BottomCenter:
                        imgleft += (padbounds.Width - imgswidth) / 2;
                        imgtop += padbounds.Height - size;
                        break;
                    case DataGridViewContentAlignment.TopRight:
                        imgleft += padbounds.Width - imgswidth;
                        break;
                    case DataGridViewContentAlignment.MiddleRight:
                        imgleft += padbounds.Width - imgswidth;
                        imgtop += (padbounds.Height - size) / 2;
                        break;
                    case DataGridViewContentAlignment.BottomRight:
                        imgleft += padbounds.Width - imgswidth;
                        imgtop += padbounds.Height - size;
                        break;
                    case DataGridViewContentAlignment.MiddleLeft:
                        imgtop += (padbounds.Height - size) / 2;
                        break;
                    case DataGridViewContentAlignment.BottomLeft:
                        imgtop += padbounds.Height - size;
                        break;
                    case DataGridViewContentAlignment.NotSet:
                    case DataGridViewContentAlignment.MiddleCenter:
                        imgleft += (padbounds.Width - imgswidth) / 2;
                        imgtop += (padbounds.Height - size) / 2;
                        break;
                    default:
                        break;
                }
                
                graphics.DrawImage(he.GetIcon, new Rectangle(imgleft, imgtop, size, size));
                imgleft += size + padding;

                if (ShowMapColourForFSD && he.IsFSDJump)
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(he.MapColour)))
                        graphics.FillEllipse(b, new Rectangle(imgleft, imgtop, size, size));
                    imgleft += size + padding;
                }

                if (he.StartMarker)
                    graphics.DrawImage(EDDiscovery.Properties.Resources.startflag, new Rectangle(imgleft, imgtop, size, size));
                else if (he.StopMarker)
                    graphics.DrawImage(EDDiscovery.Properties.Resources.stopflag, new Rectangle(imgleft, imgtop, size, size));
            }
        }

        #endregion // EventDisplayCell
    }
}
