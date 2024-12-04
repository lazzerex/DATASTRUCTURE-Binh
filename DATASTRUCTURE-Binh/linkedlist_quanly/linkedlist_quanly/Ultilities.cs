using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedPictureBox : PictureBox
{
    public int CornerRadius { get; set; } = 15; // Default corner radius
    public string DisplayText { get; set; } = string.Empty; // Text to display
    public Color TextColor { get; set; } = Color.Black; // Default text color
    public Font TextFont { get; set; } = SystemFonts.DefaultFont; // Default font for the text
    public int TextStartX { get; set; } = 0; // X position to start drawing the text

    // New properties for border customization
    public Color BorderColor { get; set; } = Color.Black; // Default border color
    public int BorderThickness { get; set; } = 2; // Default border thickness
    public bool ShowBorder { get; set; } = false; // Option to show or hide border

    public RoundedPictureBox()
    {
        this.DoubleBuffered = true; // Enable double buffering
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        // Create a graphics object for the rounded rectangle
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90); // Top-left
        path.AddArc(Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90); // Top-right
        path.AddArc(Width - CornerRadius, Height - CornerRadius, CornerRadius, CornerRadius, 0, 90); // Bottom-right
        path.AddArc(0, Height - CornerRadius, CornerRadius, CornerRadius, 90, 90); // Bottom-left
        path.CloseFigure();

        // Set the region of the PictureBox to the rounded rectangle
        this.Region = new Region(path);

        // Draw the image
        base.OnPaint(pe);

        // Draw the border if ShowBorder is true
        if (ShowBorder)
        {
            using (Pen borderPen = new Pen(BorderColor, BorderThickness))
            {
                pe.Graphics.DrawPath(borderPen, path);
            }
        }

        // Draw the text if it is not empty
        if (!string.IsNullOrEmpty(DisplayText))
        {
            // Measure the size of the text
            Size textSize = TextRenderer.MeasureText(DisplayText, TextFont);

            // Calculate the position to draw the text
            PointF textPosition = new PointF(TextStartX, (ClientRectangle.Height - textSize.Height) / 2);

            // Draw the text
            using (Brush textBrush = new SolidBrush(TextColor))
            {
                pe.Graphics.DrawString(DisplayText, TextFont, textBrush, textPosition);
            }
        }
    }
}