using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace DroneWar;

public partial class BattleField: UserControl
{
    public BattleField()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var rect = new Rect(Bounds.Size);
        context.Custom(new SKBattleField(rect));
    }

    //TODO: Pokud nebudete pouzivat Skia, ale budete pouzivat primo Avalonii, celou tuto tridu odstrante
    //a pracujte s metodou Render(DrawingContext context)
    private sealed class SKBattleField : ICustomDrawOperation
    {
        public Rect Bounds { get; }
        public SKBattleField(Rect bounds) => Bounds = bounds;

        public void Render(ImmediateDrawingContext context)
        {
            //obtain Skia canvas for our drawing
            var leaseFeature = context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) as ISkiaSharpApiLeaseFeature; //<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null) return;

            using var lease = leaseFeature.Lease();

            //from now on we use Skia graphics library to draw everything
            var canvas = lease.SkCanvas;

            canvas.Clear(SKColors.White);

            using var paint = new SKPaint { Color = SKColors.DeepSkyBlue, IsAntialias = true };
            canvas.DrawRect(100, 100, 60, 60, paint);
        }

        public bool HitTest(Point p) => false;
        public void Dispose() { }
        public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other);
    }
}