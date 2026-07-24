using Avalonia;
using Avalonia.Media;
using CrosshairPro.Core.Models;

namespace CrosshairPro.App.Helpers;

public static class CrosshairRenderer
{
    private const float PixelsPerUnit = 10f;

    public static void Render(DrawingContext context, CrosshairSettings settings, Size renderSize, double scale = 1.0)
    {
        var centerX = renderSize.Width / 2;
        var centerY = renderSize.Height / 2;

        var (r, g, b) = settings.ActualColor;
        var alpha = settings.Alpha / 255.0;

        var mainColor = new Color((byte)(255 * alpha), r, g, b);
        var outlineColor = new Color((byte)(255 * alpha), 0, 0, 0);

        var mainBrush = new SolidColorBrush(mainColor);
        var outlineBrush = new SolidColorBrush(outlineColor);

        var size = settings.Size * PixelsPerUnit * scale;
        var thickness = settings.Thickness * scale;
        var gap = settings.Gap * PixelsPerUnit * scale;
        var outlineThickness = settings.HasOutline ? settings.OutlineThickness * scale : 0;

        if (settings.HasOutline && outlineThickness > 0)
        {
            DrawCrosshairLines(context, centerX, centerY, size, thickness, gap, outlineThickness,
                outlineBrush, settings.IsTShaped);
        }

        DrawCrosshairLines(context, centerX, centerY, size, thickness, gap, 0,
            mainBrush, settings.IsTShaped);

        if (settings.HasCenterDot)
        {
            var dotSize = settings.CenterDotSize * scale;

            if (settings.HasOutline && outlineThickness > 0)
            {
                var dotPen = new Pen(outlineBrush, dotSize + outlineThickness * 2);
                context.DrawLine(dotPen,
                    new Point(centerX, centerY),
                    new Point(centerX, centerY));
            }

            var mainDotPen = new Pen(mainBrush, dotSize);
            context.DrawLine(mainDotPen,
                new Point(centerX, centerY),
                new Point(centerX, centerY));
        }
    }

    private static void DrawCrosshairLines(DrawingContext context, double cx, double cy,
        double size, double thickness, double gap, double outlineOffset,
        IBrush brush, bool isTShaped)
    {
        var totalThickness = thickness + outlineOffset * 2;
        var pen = new Pen(brush, totalThickness);

        var leftStart = cx - gap - size;
        var leftEnd = cx - gap;
        var rightStart = cx + gap;
        var rightEnd = cx + gap + size;

        var topStart = cy - gap - size;
        var topEnd = cy - gap;
        var bottomStart = cy + gap;
        var bottomEnd = cy + gap + size;

        context.DrawLine(pen,
            new Point(leftStart, cy),
            new Point(leftEnd, cy));

        context.DrawLine(pen,
            new Point(rightStart, cy),
            new Point(rightEnd, cy));

        if (!isTShaped)
        {
            context.DrawLine(pen,
                new Point(cx, topStart),
                new Point(cx, topEnd));
        }

        context.DrawLine(pen,
            new Point(cx, bottomStart),
            new Point(cx, bottomEnd));
    }
}
