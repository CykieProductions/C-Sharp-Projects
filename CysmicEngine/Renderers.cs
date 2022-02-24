using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CysmicEngine
{
    public abstract class Renderer : Component
    {
        public int sortOrder = 0;
        //protected bool _isGizmo = false;
        //public bool isGizmo { get { return _isGizmo; } }

        public abstract void Draw(Graphics graphics);
    }

    public class Shape2D : Renderer
    {
        Vector2 pos;
        Vector2 scale;
        public Vector2 offset;
        public Vector2 size;
        //public Vector2 offset;
        public Color color;
        bool isFilled = true;

        public enum ShapeType
        {
            Rectangle, Circle
        }
        public ShapeType shape = ShapeType.Rectangle;

        void _MasterConstructor(Color clr, Vector2 _size, Vector2 _offset, ShapeType shapeType = ShapeType.Rectangle, bool fill = true, int srtOdr = 0)
        {
            sortOrder = srtOdr;
            size = _size;
            color = clr;
            shape = shapeType;
            isFilled = fill;
            offset = _offset;
        }
        public Shape2D(Color clr, ShapeType shapeType = ShapeType.Rectangle, bool fill = true, int srtOdr = 0)
        {
            _MasterConstructor(clr, (1, 1), Vector2.Zero, shapeType, fill, srtOdr);
        }
        public Shape2D(Color clr, Vector2 size, Vector2 offset, ShapeType shapeType = ShapeType.Rectangle, bool fill = true, int srtOdr = 0)
        {
            _MasterConstructor(clr, size, offset, shapeType, fill, srtOdr);
        }

        /*protected override void Update()
        {
        }*/

        public override void Draw(Graphics graphics)
        {
            scale = transform.scale * size;
            pos = transform.position + offset;//(transform.position.x + (scale.x / 2), transform.position.y/* + (scale.y / 2)*/);
            SolidBrush brush = new SolidBrush(color);

            switch (shape)
            {
                case ShapeType.Rectangle:
                    //graphics.FillRectangle(Brushes.Blue, pos.x - (size.x / 2), pos.y + (size.y / 2), size.x, size.y);
                    if(isFilled)
                        graphics.FillRectangle(brush, pos.x, pos.y, scale.x, scale.y);
                    else
                        graphics.DrawRectangle(new Pen(brush, 2.5f), pos.x, pos.y, scale.x, scale.y);
                    break;

                case ShapeType.Circle:
                    graphics.FillEllipse(brush, pos.x, pos.y, scale.x, scale.y);
                    break;

                default:
                    break;
            }
        }
    }

    public class SpriteRenderer : Renderer
    {
        public string spritePath;
        string fullPath;
        public Bitmap sprite = null;

        public SpriteRenderer(string path, int srtOdr = 0)
        {
            spritePath = path.Replace("/", @"\");
            fullPath = ($"Assets/Sprites/{spritePath}.png").Replace("/", @"\");
            sprite = new Bitmap(Image.FromFile(fullPath));
            sortOrder = srtOdr;
        }

        public override void Draw(Graphics graphics)
        {
            try
            {
                graphics.DrawImage(sprite, transform.position.x, transform.position.y, (int)(sprite.Width * transform.scale.x), (int)(sprite.Height * transform.scale.y));
            }
            catch
            {

            }
        }
    }
}
