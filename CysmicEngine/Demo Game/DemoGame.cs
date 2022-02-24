using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CysmicEngine.Demo_Game
{
    class DemoGame : CysmicGame
    {
        public Transform player;
        public Transform floor;
        //public Transform camGizmo;

        public DemoGame() : base(new Vector2(800, 450), "Cysmic Engine Demo")
        {

        }
        DateTime prevNow;
        public override void OnStart()
        {
            prevNow = DateTime.UtcNow;
            player = new GameObject("Player", trnfrm: new Transform
                ((game.window.Size.Width / 2, (game.window.Size.Height / 2) - 50), (1, 1), 0)
                , components: new List<Component>()
            {
                new SpriteRenderer("Main Characters/Virtual Guy/Fall (32x32)", 50),
                new PlayerMotor(),
                new InputController(),
                new Rigidbody2D(),
                new Collider2D(os: (2, 2) , (28, 28)),
            }
            ).transform;
            player.transform.isStatic = false;
            player.isStatic = false;
            if (player.TryGetComponent(out Rigidbody2D someRB))
            {
                someRB.gravScale = 3;
                //someRB.isPushable = false;
            }

            var box = new GameObject("Box", trnfrm: new Transform
                ((game.window.Size.Width / 2, (game.window.Size.Height / 2) - 90), (1, 1), 0)
                , components: new List<Component>()
            {
                //new SpriteRenderer("Main Characters/Virtual Guy/Fall (32x32)", 50),
                new Shape2D(Color.Red, size: (32,32), offset: Vector2.Zero, srtOdr: -5),
                //new PlayerMotor(),
                //new InputController(),
                new Rigidbody2D(),
                new Collider2D((0, 0), (32, 32), isTrig: false),
                //new Collider2D(Vector2.Zero, (10, 10)),
            }, lyr: "Ground"
            ).transform;
            box.transform.isStatic = false;
            box.isStatic = false;
            if (box.TryGetComponent(out someRB))
            {
                someRB.gravScale = 3;
            }

            floor = new GameObject("Floor", trnfrm: new Transform
                (pos: ((game.window.Size.Width / 2) - 200, (game.window.Size.Height / 2) + 50), scl: (300, 30), rot: 0)
                , components: new List<Component>()
            {
                new Shape2D(Color.Blue, size: (1, 1), offset: Vector2.Zero, Shape2D.ShapeType.Rectangle),
                new Collider2D(Vector2.Zero,(1,1))
            }, lyr: "Ground"
            ).transform;
            
            floor = new GameObject("Floor", trnfrm: new Transform
                (pos: ((game.window.Size.Width / 2) - 60 - 200, (game.window.Size.Height / 2) + 130), scl: (420, 30), rot: 0)
                , components: new List<Component>()
            {
                new Shape2D(Color.Blue, size: (1, 1), offset: Vector2.Zero, Shape2D.ShapeType.Rectangle),
                new Collider2D(Vector2.Zero,(1,1))
            }, lyr: "Ground"
            ).transform;

            floor = new GameObject("Floor", trnfrm: new Transform
                (pos: ((game.window.Size.Width / 2) - 60, (game.window.Size.Height / 2) - 50), scl: (20, 300), rot: 0)
                , components: new List<Component>()
            {
                new Shape2D(Color.Blue, size: (1, 1), offset: Vector2.Zero, Shape2D.ShapeType.Rectangle),
                new Collider2D(Vector2.Zero,(1,1))
            }, lyr: "Ground"
            ).transform;

            /*new GameObject("Cam Gizmo BR", components: new List<Component>()
            {
                new Shape2D(Color.Purple, sz: (10, 10), Shape2D.ShapeType.Circle)
            }).transform.SetPosition(((instance.window.Size.Width / 2) - 50, (instance.window.Size.Height / 2) + 50));*/

            /*camGizmo = new GameObject("Cam Gizmo BR", components: new List<Component>()
            {
                new Shape2D(Color.Blue, sz: (10, 10), Shape2D.ShapeType.Circle)
            }
            ).transform.SetPosition((Cam.VisibleClipBounds.Right, Cam.VisibleClipBounds.Bottom)).SetScale((25, 25));
            camGizmo = new GameObject("Cam Gizmo TR", components: new List<Component>()
            {
                new Shape2D(Color.Blue, sz: (10, 10), Shape2D.ShapeType.Circle)
            }
            ).transform.SetPosition((Cam.VisibleClipBounds.Right, Cam.VisibleClipBounds.Top)).SetScale((25, 25));
            camGizmo = new GameObject("Cam Gizmo BL", components: new List<Component>()
            {
                new Shape2D(Color.Blue, sz: (10, 10), Shape2D.ShapeType.Circle)
            }
            ).transform.SetPosition((Cam.VisibleClipBounds.Left, Cam.VisibleClipBounds.Bottom)).SetScale((25, 25));
            camGizmo = new GameObject("Cam Gizmo TL", components: new List<Component>()
            {
                new Shape2D(Color.Blue, sz: (10, 10), Shape2D.ShapeType.Circle)
            }
            ).transform.SetPosition((Cam.VisibleClipBounds.Left, Cam.VisibleClipBounds.Top)).SetScale((25, 25));*/
        }

        public override void Update()
        {
            base.Update();
            /*camGizmo.transform.position.x = Cam.position.x;
            camGizmo.transform.position.y = Cam.position.y;*/
            //player.scale.x += deltaTime;

            /*if (Input.PressedKey(Keys.Up))
                print("Player pressed: " + Keys.Up);
            if (Input.HoldingKey(Keys.Up))
                print("Player is holding down: " + Keys.Up);
            if (Input.ReleasedKey(Keys.Up))
                print("Player released: " + Keys.Up);*/

            //print(Input.GetAxis(AxisName.HORIZONTAL));
        }
        float timer = 0;
        int myFrames = 0;
        int trueFrames = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();
            //CountFPS();

            /*//Vector2 camPos = (graphics.DpiX / 2, graphics.DpiY / 2);
            Vector2 camPos = Cam.position;
            Vector2 targetPos = ((window.Width / 2), (window.Height / 2));

            if (Vector2.Distance(Cam.position, targetPos) > 0.2f)
                print("Player Pos: " + targetPos + " | Cam Pos: " + camPos);

            if (Math.Abs(camPos.x - targetPos.x) > 0.2f)
            {
                if (camPos.x < targetPos.x)//too far left
                    Cam.position.x += deltaTime * Cam.speed;//so move right
                else
                    Cam.position.x += deltaTime * -Cam.speed;
            }

            if (Math.Abs(camPos.y - targetPos.y) > 0.2f)
            {
                if (camPos.y < targetPos.y)//too far down
                    Cam.position.y += deltaTime * Cam.speed;//so move up
                else
                    Cam.position.y += deltaTime * -Cam.speed;
            }*/
        }

        void CountFPS(bool showTrueFPS = true)
        {
            if (timer >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                //print("My Timer: " + timer);
                print("My FPS: " + myFrames);
                Console.ForegroundColor = ConsoleColor.White;
                timer = 0;
                myFrames = 0;
            }
            timer += Time.deltaTime;
            //print("My Timer: " + timer);
            myFrames++;

            if (showTrueFPS && DateTime.UtcNow.Subtract(prevNow).TotalSeconds >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                //print("True Timer: " + DateTime.UtcNow.Subtract(prevNow).TotalSeconds);
                print("True FPS: " + trueFrames);
                Console.ForegroundColor = ConsoleColor.White;
                trueFrames = 0;
                prevNow = DateTime.UtcNow;
            }
            trueFrames++;
        }

        float fixedTimer = 0;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ////*print("Fixed Timer: " + fixedTimer);
            if(fixedTimer * Time.fixedDeltaTime >= 1)
            {
                //print("Fixed Second");
                fixedTimer = 0;
            }
            fixedTimer++;//*/
        }
    }

}
