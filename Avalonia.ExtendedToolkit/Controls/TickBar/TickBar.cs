﻿using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.ExtendedToolkit.Extensions;
using Avalonia.Media;
using Avalonia.Reactive;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.ExtendedToolkit.Controls
{
    public class TickBar : Control
    {
        public IBrush Fill
        {
            get { return (IBrush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }


        public static readonly AvaloniaProperty FillProperty =
            AvaloniaProperty.Register<TickBar, IBrush>(nameof(Fill));



        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        public static readonly AvaloniaProperty MinimumProperty =
            RangeBase.MinimumProperty.AddOwner<TickBar>(x => x.Minimum,
                (x, y) => x.Minimum = y, unsetValue: 0.0);



        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        public static readonly AvaloniaProperty MaximumProperty =
            RangeBase.MaximumProperty.AddOwner<TickBar>(x => x.Maximum,
                (x, y) => x.Maximum = y, unsetValue: 100.0);





        public double SelectionStart
        {
            get { return (double)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }


        public static readonly AvaloniaProperty SelectionStartProperty =
            AvaloniaProperty.Register<TickBar, double>(nameof(SelectionStart), defaultValue: -1.0d);
        //slider does not have a selectionstart property



        public double SelectionEnd
        {
            get { return (double)GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }


        public static readonly AvaloniaProperty SelectionEndProperty =
            AvaloniaProperty.Register<TickBar, double>(nameof(SelectionEnd), defaultValue: -1.0d);



        public bool IsSelectionRangeEnabled
        {
            get { return (bool)GetValue(IsSelectionRangeEnabledProperty); }
            set { SetValue(IsSelectionRangeEnabledProperty, value); }
        }


        public static readonly AvaloniaProperty IsSelectionRangeEnabledProperty =
            AvaloniaProperty.Register<TickBar, bool>(nameof(IsSelectionRangeEnabled));



        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }


        public static readonly AvaloniaProperty TickFrequencyProperty =
            Slider.TickFrequencyProperty.AddOwner<TickBar>();

        //Ticks missing



        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }


        public static readonly AvaloniaProperty IsDirectionReversedProperty =
            Track.IsDirectionReversedProperty.AddOwner<TickBar>();



        public TickBarPlacement Placement
        {
            get { return (TickBarPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }


        public static readonly AvaloniaProperty PlacementProperty =
            AvaloniaProperty.Register<TickBar, TickBarPlacement>(nameof(Placement),
                defaultValue: TickBarPlacement.Top);



        /// <summary>
        /// TickBar will use ReservedSpaceProperty for left and right spacing (for horizontal orientation) or
        /// tob and bottom spacing (for vertical orienation).
        /// The space on both sides of TickBar is half of specified ReservedSpace.
        /// This property has type of <see cref="double" />.
        /// </summary>
        public double ReservedSpace
        {
            get { return (double)GetValue(ReservedSpaceProperty); }
            set { SetValue(ReservedSpaceProperty, value); }
        }


        public static readonly AvaloniaProperty ReservedSpaceProperty =
            AvaloniaProperty.Register<TickBar, double>(nameof(ReservedSpace), defaultValue: 0d);


        public override void Render(DrawingContext dc)
        {
            Size size = new Size(Width, Height);
            double range = Maximum - Minimum;
            double tickLen = 0.0d;  // Height for Primary Tick (for Mininum and Maximum value)
            double tickLen2;        // Height for Secondary Tick
            double logicalToPhysical = 1.0;
            double progression = 1.0d;
            Point startPoint = new Point(0d, 0d);
            Point endPoint = new Point(0d, 0d);

            // Take Thumb size in to account
            double halfReservedSpace = ReservedSpace * 0.5;

            switch (Placement)
            {
                case TickBarPlacement.Top:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size = new Size(size.Width - ReservedSpace, size.Height);
                    tickLen = -size.Height;
                    startPoint = new Point(halfReservedSpace, size.Height);
                    endPoint = new Point(halfReservedSpace + size.Width, size.Height);
                    logicalToPhysical = size.Width / range;
                    progression = 1;
                    break;

                case TickBarPlacement.Bottom:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size = new Size(size.Width - ReservedSpace, size.Height);
                    tickLen = size.Height;
                    startPoint = new Point(halfReservedSpace, 0d);
                    endPoint = new Point(halfReservedSpace + size.Width, 0d);
                    logicalToPhysical = size.Width / range;
                    progression = 1;
                    break;

                case TickBarPlacement.Left:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size = new Size(size.Width, size.Height - ReservedSpace);
                    tickLen = -size.Width;
                    startPoint = new Point(size.Width, size.Height + halfReservedSpace);
                    endPoint = new Point(size.Width, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    progression = -1;
                    break;

                case TickBarPlacement.Right:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size = new Size(size.Width, size.Height - ReservedSpace);
                    tickLen = size.Width;
                    startPoint = new Point(0d, size.Height + halfReservedSpace);
                    endPoint = new Point(0d, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    progression = -1;
                    break;
            };

            tickLen2 = tickLen * 0.75;

            // Invert direciton of the ticks
            if (IsDirectionReversed)
            {
                progression = -progression;
                logicalToPhysical *= -1;

                // swap startPoint & endPoint
                Point pt = startPoint;
                startPoint = endPoint;
                endPoint = pt;
            }

            Pen pen = new Pen(Fill, 1.0d);

            //bool snapsToDevicePixels = SnapsToDevicePixels;
            //DoubleCollection xLines = snapsToDevicePixels ? new DoubleCollection() : null;
            //DoubleCollection yLines = snapsToDevicePixels ? new DoubleCollection() : null;

            // Is it Vertical?
            if ((Placement == TickBarPlacement.Left) || (Placement == TickBarPlacement.Right))
            {
                // Reduce tick interval if it is more than would be visible on the screen
                double interval = TickFrequency;
                if (interval > 0.0)
                {
                    double minInterval = (Maximum - Minimum) / size.Height;
                    if (interval < minInterval)
                    {
                        interval = minInterval;
                    }
                }

                // Draw Min & Max tick
                dc.DrawLine(pen, startPoint, new Point(startPoint.X + tickLen, startPoint.Y));
                dc.DrawLine(pen, new Point(startPoint.X, endPoint.Y),
                                 new Point(startPoint.X + tickLen, endPoint.Y));

                //if (snapsToDevicePixels)
                //{
                //    xLines.Add(startPoint.X);
                //    yLines.Add(startPoint.Y - 0.5);
                //    xLines.Add(startPoint.X + tickLen);
                //    yLines.Add(endPoint.Y - 0.5);
                //    xLines.Add(startPoint.X + tickLen2);
                //}

                // This property is rarely set so let's try to avoid the GetValue
                // caching of the mutable default value
                //DoubleCollection ticks = null;
                //bool hasModifiers;
                //if (GetValueSource(TicksProperty, null, out hasModifiers)
                //    != BaseValueSourceInternal.Default || hasModifiers)
                //{
                //    ticks = Ticks;
                //}

                // Draw ticks using specified Ticks collection
                //if ((ticks != null) && (ticks.Count > 0))
                //{
                //    for (int i = 0; i < ticks.Count; i++)
                //    {
                //        if (DoubleUtil.LessThanOrClose(ticks[i], Minimum) || DoubleUtil.GreaterThanOrClose(ticks[i], Maximum))
                //        {
                //            continue;
                //        }

                //        double adjustedTick = ticks[i] - Minimum;

                //        double y = adjustedTick * logicalToPhysical + startPoint.Y;
                //        dc.DrawLine(pen,
                //            new Point(startPoint.X, y),
                //            new Point(startPoint.X + tickLen2, y));

                //        if (snapsToDevicePixels)
                //        {
                //            yLines.Add(y - 0.5);
                //        }
                //    }
                //}
                // Draw ticks using specified TickFrequency
                //else if (interval > 0.0)
                //{
                //    for (double i = interval; i < range; i += interval)
                //    {
                //        double y = i * logicalToPhysical + startPoint.Y;

                //        dc.DrawLine(pen,
                //            new Point(startPoint.X, y),
                //            new Point(startPoint.X + tickLen2, y));

                //        //if (snapsToDevicePixels)
                //        //{
                //        //    yLines.Add(y - 0.5);
                //        //}
                //    }
                //}

                // Draw Selection Ticks
                if (IsSelectionRangeEnabled)
                {
                    double y0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.Y;
                    Point pt0 = new Point(startPoint.X, y0);
                    Point pt1 = new Point(startPoint.X + tickLen2, y0);
                    Point pt2 = new Point(startPoint.X + tickLen2, y0 + Math.Abs(tickLen2) * progression);

                    //PathSegment[] segments = new PathSegment[] {
                    //    //new LineSegment(pt2, true),
                    //    //new LineSegment(pt0, true),
                    //    new LineSegment{Point=pt2 },
                    //    new LineSegment{Point=pt0 },
                    //};

                    PathSegments segments = new PathSegments();
                    segments.Add(new LineSegment { Point = pt2 });
                    segments.Add(new LineSegment { Point = pt0 });


                    PathGeometry geo = new PathGeometry { Figures = new PathFigures { new PathFigure { StartPoint = pt1, Segments = segments, IsClosed = true } } };

                    dc.DrawGeometry(Fill, pen, geo);

                    y0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.Y;
                    pt0 = new Point(startPoint.X, y0);
                    pt1 = new Point(startPoint.X + tickLen2, y0);
                    pt2 = new Point(startPoint.X + tickLen2, y0 - Math.Abs(tickLen2) * progression);

                    segments = new PathSegments();
                    segments.Add(new LineSegment { Point = pt2 });
                    segments.Add(new LineSegment { Point = pt0 });


                    geo = new PathGeometry { Figures = new PathFigures { new PathFigure { StartPoint = pt1, Segments = segments, IsClosed = true } } };
                    dc.DrawGeometry(Fill, pen, geo);
                }
            }
            else  // Placement == Top || Placement == Bottom
            {
                // Reduce tick interval if it is more than would be visible on the screen
                double interval = TickFrequency;
                if (interval > 0.0)
                {
                    double minInterval = (Maximum - Minimum) / size.Width;
                    if (interval < minInterval)
                    {
                        interval = minInterval;
                    }
                }

                // Draw Min & Max tick
                dc.DrawLine(pen, startPoint, new Point(startPoint.X, startPoint.Y + tickLen));
                dc.DrawLine(pen, new Point(endPoint.X, startPoint.Y),
                                 new Point(endPoint.X, startPoint.Y + tickLen));

                //if (snapsToDevicePixels)
                //{
                //    xLines.Add(startPoint.X - 0.5);
                //    yLines.Add(startPoint.Y);
                //    xLines.Add(startPoint.X - 0.5);
                //    yLines.Add(endPoint.Y + tickLen);
                //    yLines.Add(endPoint.Y + tickLen2);
                //}

                // This property is rarely set so let's try to avoid the GetValue
                // caching of the mutable default value
                //DoubleCollection ticks = null;
                //bool hasModifiers;
                //if (GetValueSource(TicksProperty, null, out hasModifiers)
                //    != BaseValueSourceInternal.Default || hasModifiers)
                //{
                //    ticks = Ticks;
                //}

                // Draw ticks using specified Ticks collection
                //if ((ticks != null) && (ticks.Count > 0))
                //{
                //    for (int i = 0; i < ticks.Count; i++)
                //    {
                //        if (DoubleUtil.LessThanOrClose(ticks[i], Minimum) || DoubleUtil.GreaterThanOrClose(ticks[i], Maximum))
                //        {
                //            continue;
                //        }
                //        double adjustedTick = ticks[i] - Minimum;

                //        double x = adjustedTick * logicalToPhysical + startPoint.X;
                //        dc.DrawLine(pen,
                //            new Point(x, startPoint.Y),
                //            new Point(x, startPoint.Y + tickLen2));

                //        if (snapsToDevicePixels)
                //        {
                //            xLines.Add(x - 0.5);
                //        }
                //    }
            }
            // Draw ticks using specified TickFrequency
            //else if (interval > 0.0)
            //{
            //    for (double i = interval; i < range; i += interval)
            //    {
            //        double x = i * logicalToPhysical + startPoint.X;
            //        dc.DrawLine(pen,
            //            new Point(x, startPoint.Y),
            //            new Point(x, startPoint.Y + tickLen2));

            //        if (snapsToDevicePixels)
            //        {
            //            xLines.Add(x - 0.5);
            //        }
            //    }
            //}

            // Draw Selection Ticks
            if (IsSelectionRangeEnabled)
            {
                double x0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.X;
                Point pt0 = new Point(x0, startPoint.Y);
                Point pt1 = new Point(x0, startPoint.Y + tickLen2);
                Point pt2 = new Point(x0 + Math.Abs(tickLen2) * progression, startPoint.Y + tickLen2);

                //PathSegment[] segments = new PathSegment[] {
                //    new LineSegment(pt2, true),
                //    new LineSegment(pt0, true),
                //};

                PathSegments segments = new PathSegments();
                segments.Add(new LineSegment { Point = pt2 });
                segments.Add(new LineSegment { Point = pt0 });

                PathGeometry geo = new PathGeometry { Figures = new PathFigures { new PathFigure { StartPoint = pt1, Segments = segments, IsClosed = true } } };

                dc.DrawGeometry(Fill, pen, geo);

                x0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.X;
                pt0 = new Point(x0, startPoint.Y);
                pt1 = new Point(x0, startPoint.Y + tickLen2);
                pt2 = new Point(x0 - Math.Abs(tickLen2) * progression, startPoint.Y + tickLen2);

                segments = new PathSegments();
                segments.Add(new LineSegment { Point = pt2 });
                segments.Add(new LineSegment { Point = pt0 });

                geo = new PathGeometry { Figures = new PathFigures { new PathFigure { StartPoint = pt1, Segments = segments, IsClosed = true } } };
                dc.DrawGeometry(Fill, pen, geo);
            }
            //}

            //if (snapsToDevicePixels)
            //{
            //    xLines.Add(ActualWidth);
            //    yLines.Add(ActualHeight);
            //    VisualXSnappingGuidelines = xLines;
            //    VisualYSnappingGuidelines = yLines;
            //}
            return;


        }

        private void BindToTemplatedParent(AvaloniaProperty target, AvaloniaProperty source)
        {
            //if (!HasNonDefaultValue(target))
            {
                //Binding binding = new Binding();
                //binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                //binding.Path = source.p//new PropertyPath(source);
                //SetBinding(target, binding);
                Bind(target, ObservableEx.SingleValue(source));

            }
        }


        public override void ApplyTemplate()
        {
            base.ApplyTemplate();

            Slider parent = TemplatedParent as Slider;
            Track track = parent?.FindChildren<Track>();


            if (parent != null)
            {
                //BindToTemplatedParent(TicksProperty, Slider.TicksProperty);
                //BindToTemplatedParent(TickFrequencyProperty, Slider.TickFrequencyProperty);
                //BindToTemplatedParent(IsSelectionRangeEnabledProperty, Slider.IsSelectionRangeEnabledProperty);
                //BindToTemplatedParent(SelectionStartProperty, Slider.SelectionStartProperty);
                //BindToTemplatedParent(SelectionEndProperty, Slider.SelectionEndProperty);
                BindToTemplatedParent(MinimumProperty, Slider.MinimumProperty);
                BindToTemplatedParent(MaximumProperty, Slider.MaximumProperty);
                BindToTemplatedParent(IsDirectionReversedProperty, Track.IsDirectionReversedProperty);

                if (/*!HasNonDefaultValue(ReservedSpaceProperty) &&*/ track != null)
                {
                    Binding binding = new Binding();
                    //binding.Source = track.Thumb;

                    if (parent.Orientation == Layout.Orientation.Horizontal)
                    {
                        //binding.Path = new PropertyPath(Thumb.ActualWidthProperty);
                        Bind(ReservedSpaceProperty, ObservableEx.SingleValue(Thumb.WidthProperty));
                    }
                    else
                    {
                        //binding.Path = new PropertyPath(Thumb.ActualHeightProperty);
                        Bind(ReservedSpaceProperty, ObservableEx.SingleValue(Thumb.HeightProperty));
                    }


                    //SetBinding(ReservedSpaceProperty, binding);
                }
            }



        }







    }
}
