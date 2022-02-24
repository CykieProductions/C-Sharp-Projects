using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using CyTools;
using static CyTools.Basic;

namespace CysmicEngine
{
    public class Canvas : Form
    {
        public Canvas()
        {
            DoubleBuffered = true;
        }
    }


    public enum AxisName
    {
        HORIZONTAL, VERTICAL
    }
    public static class Input
    {
        public static bool pressedOnThisFrame = true;
        public static bool releasedOnThisFrame = true;
        static HashSet<Keys> curPressedKeys = new HashSet<Keys>();
        static HashSet<Keys> justPressedKeys = new HashSet<Keys>();
        static HashSet<Keys> justReleasedKeys = new HashSet<Keys>();

        static Dictionary<string, (Keys, Keys)> axes = new Dictionary<string, (Keys, Keys)>()
        {
            ["Horizontal"] = (Keys.A,Keys.D),//(-1, 1)
            ["horizontal"] = (Keys.Left,Keys.Right),
            ["Vertical"] = (Keys.S,Keys.W),
            ["vertical"] = (Keys.Down,Keys.Up),
        };

        internal static void Win_KD_Event(object sender, KeyEventArgs e)
        {
            /*if(e.KeyCode == Keys.Space)
                print("Jump Key Down Event!");*/
            if(!curPressedKeys.Contains(e.KeyCode))//ensure it fires only once per press
                justPressedKeys.Add(e.KeyCode);
            curPressedKeys.Add(e.KeyCode);
            pressedOnThisFrame = false;
            //releasedOnThisFrame = false;
        }
        internal static void Win_KU_Event(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.Space)
                print("Jump Key Up Event!");*/

            curPressedKeys.Remove(e.KeyCode);
            justReleasedKeys.Add(e.KeyCode);
            //pressedOnThisFrame = false;
            releasedOnThisFrame = false;
        }
        /// <summary>Clear the list of one time presses at the end of every frame</summary>
        internal static void ClearPressedKeys()
        {
            /*if (justPressedKeys.Contains(Keys.Space))
                print("CLEAR PRESSED");*/

            justPressedKeys.Clear();
            //justReleasedKeys.Clear();
        }
        /// <summary>Clear the list of one time presses at the end of every frame</summary>
        internal static void ClearReleasedKeys()
        {
            /*if (justReleasedKeys.Contains(Keys.Space))
                print("CLEAR RELEASED");*/

            //justPressedKeys.Clear();
            justReleasedKeys.Clear();
        }

        public static bool PressedKey(Keys key)
        {
            /*if (key == Keys.Space && justPressedKeys.Contains(key))
                print("Pressed the Jump Key!");*/
            pressedOnThisFrame = true;
            return justPressedKeys.Contains(key);
        }
        /// <summary>Is this key being held down?</summary>
        /// <param name="includeFirstFrame">If false, this won't return true on the first frame of the press</param>
        public static bool HoldingKey(Keys key, bool includeFirstFrame = true)
        {
            if(includeFirstFrame)
                return curPressedKeys.Contains(key);
            else
                return !justPressedKeys.Contains(key) && curPressedKeys.Contains(key);
        }
        public static bool ReleasedKey(Keys key)//Note: it's possible to trigger this without triggering KeyDown first
        {
            /*if (key == Keys.Space && justReleasedKeys.Contains(key))
                print("Released the Jump Key!");*/
            releasedOnThisFrame = true;
            return justReleasedKeys.Contains(key);
        }

        public static int GetAxis(AxisName name)
        {
            List<string> altNames = new List<string>();
            for (int i = 0; i < axes.Keys.Count; i++)
            {
                if (name.ToString().ToUpper() == axes.Keys.ElementAt(i).ToUpper())
                {
                    altNames.Add(axes.Keys.ElementAt(i));
                }
            }
            if (altNames.Count == 0)//name didn't match with any alternate capitalizing
                throw new Exception("The provided Axis name wasn't valid");

            for (int i = 0; i < altNames.Count; i++)
            {
                if (!axes.TryGetValue(altNames[i], out (Keys, Keys) axis))
                    continue;

                if (curPressedKeys.Contains(axis.Item1))
                    return -1;
                else if (curPressedKeys.Contains(axis.Item2))
                    return 1;
            }
            return 0;//name was valid, but nothing was pressed
        }
    }

    public static class Cam
    {
        public static Vector2 position = Vector2.Zero;
        static Vector2 _camCenter = Vector2.Zero;
        static public Vector2 camCenter { get { return _camCenter; } }

        public static float speed = 120;
        static RectangleF _VisibleClipBounds = new RectangleF();
        public static RectangleF VisibleClipBounds { get { return _VisibleClipBounds; } }

        internal static void SetCenter(Canvas window)
        {
            //_camCenter = (position.x + graphics.DpiX / 2, position.y + graphics.DpiY / 2);
            _camCenter = (position.x + window.Width / 2, position.y + window.Height / 2);
        }
        internal static void GetVisibleClipBounds(Graphics graphics)
        {
            _VisibleClipBounds = graphics.ClipBounds;
        }
    }

    public class CE_Common : Basic
    {
        public float Lerp(float start, float end, float percentage)
        {
            return start + percentage * (end - start);
        }
    }

    public static class Time
    {
        public static DateTime prevFrameTime = DateTime.Today;
        public const float fixedDeltaTime = 0.02f;
        public const float targetFPS = 60;
        static float _deltaTime = 0;
        public static float deltaTime { get { return _deltaTime; } }

        /// <summary>Only for use in the main Game Loop function</summary>
        public static void private_SetDT(float newDT)
        {
            _deltaTime = newDT;
        }
    }

    public abstract class CysmicGame : Basic
    {
        public static bool displayGizmos = true;

        Vector2 windowSize = new Vector2(512, 512);
        string title = "New Game";
        public Canvas window = null;
        Thread GameLoopThread;
        Thread PhysicsThread;
        InterpolationMode drawMode;

        public static List<GameObject> allGameObjects = new List<GameObject>();
        public static HashSet<GizmoObj> allGizmos = new HashSet<GizmoObj>();
        public static HashSet<Renderer> allRenderers = new HashSet<Renderer>();
        public static HashSet<Collider2D> allColliders = new HashSet<Collider2D>();

        public static CysmicGame game;

        public void Play()
        {
            Time.prevFrameTime = DateTime.MinValue;
            //window.Paint += InitGraphics;//Render Function will be added later to paint later
            window.Paint += Render;
            window.KeyDown += Input.Win_KD_Event;
            window.KeyUp += Input.Win_KU_Event;
            window.FormClosing += Window_FormClosing;
            window.FormClosed += Window_FormClosed;

            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
            //Start fixed loop after first frame of game loop

            game = this;
            Application.Run(window);
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            allRenderers.Clear();
            for (int i = 0; i < allGameObjects.Count; i++)
            {
                allGameObjects[i] = null;
            }
            allGameObjects.Clear();
        }
        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameLoopThread.Abort();
            PhysicsThread.Abort();
            Application.Exit();
        }

        public CysmicGame(Vector2 winSize, string t, InterpolationMode im = InterpolationMode.NearestNeighbor)
        {
            windowSize = winSize;
            title = t;
            drawMode = im;

            window = new Canvas();
            window.Text = title;
            window.Size = new Size((int)windowSize.x, (int)windowSize.y);
            //isFullySetUp = false;
        }

        private void FixedLoop()
        {
            while (PhysicsThread.IsAlive)
            {
                FixedUpdate();

                Thread.Sleep(TimeSpan.FromSeconds(Time.fixedDeltaTime));
            }
        }

        private void GameLoop()
        {
            OnStart();
            float desiredFrameTime = 1f / Time.targetFPS;
            while (GameLoopThread.IsAlive)
            {
                try
                {
                    if (Time.prevFrameTime == DateTime.MinValue)//update every "deltaTime" seconds
                    {
                        Time.prevFrameTime = DateTime.UtcNow;
                        if (TimeSpan.FromSeconds(desiredFrameTime - Time.deltaTime).TotalSeconds >= 0)
                        {
                            float frameOffset = (float)TimeSpan.FromSeconds(desiredFrameTime - Time.deltaTime).TotalSeconds;
                            //print($"deltaTime ({_deltaTime}) + frameOffset ({frameOffset})" +
                              //  $" = {_deltaTime + frameOffset}.");
                            Thread.Sleep(TimeSpan.FromSeconds(frameOffset));
                        }
                        else
                            Thread.Sleep(1);//some delay is needed to avoid potential crashes

                        Update();

                        var asyncDraw = window.BeginInvoke((MethodInvoker)delegate { window.Refresh(); });//Draw Frame

                        while (!asyncDraw.IsCompleted);

                        LateUpdate();

                        //Calculate deltaTime
                        var newFrameTime = DateTime.UtcNow;
                        Time.private_SetDT( (float)newFrameTime.Subtract(Time.prevFrameTime).TotalSeconds) ;
                        Time.prevFrameTime = DateTime.MinValue;
                    }
                    else
                    {
                        Time.prevFrameTime = DateTime.MinValue;
                    }
                }
                catch
                {
                    print("Game is loading...");
                }

                //Input.ClearReleasedKeys();//moved to  late update
                if (PhysicsThread == null)
                {
                    PhysicsThread = new Thread(FixedLoop);
                    PhysicsThread.Start();
                }
            }
            //if the game loop ever stops then close the game
            Application.Exit();
        }

        private void Render(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Color.Beige);
            /*if (!isFullySetUp)
                return;*/

            graphics.TranslateTransform(Cam.position.x, Cam.position.y);
            //_camCenter = (graphics.DpiX / 2, graphics.DpiY / 2);
            Cam.SetCenter(window);

            graphics.InterpolationMode = drawMode;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            allRenderers = allRenderers.OrderBy(x => x.sortOrder).ToHashSet();
            /*if(false)
            {
                allRenderers.RemoveWhere(x => x.isGizmo == true);
            }*/

            try
            {
                foreach (var renderer in allRenderers)
                {
                    renderer.Draw(graphics);
                }

                if (displayGizmos)
                {
                    foreach (var gizmo in allGizmos)
                    {
                        gizmo.renderer.Draw(graphics);
                    }
                }
            }
            catch
            {
                print("FRAME COULDN'T BE DRAWN");
                //graphics.Clear(Color.Beige);
            }
        }

        public GameObject Duplicate(GameObject gameObject)
        {
            return new GameObject(gameObject);
        }
        public static void Destroy(GameObject gameObject)
        {
            try
            {
                for (int i = 0; i < gameObject.allComponents.Count; i++)
                {
                    game.OnUpdate -= gameObject.allComponents[i].OnUpdate;
                    game.OnLateUpdate -= gameObject.allComponents[i].OnLateUpdate;
                    game.OnFixedUpdate -= gameObject.allComponents[i].OnFixedUpdate;
                    gameObject.allComponents[i].gameObject._wasDestroyed = true;
                    gameObject.allComponents[i] = null;
                }

                allGameObjects[allGameObjects.IndexOf(gameObject)] = null;
                gameObject = null;
                ClearNullGameObjects();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Called once, right before the game starts
        /// </summary>
        public abstract void OnStart();

        public static void ClearNullGameObjects()
        {
            allGameObjects.RemoveAll(x => x == null || x.wasDestroyed == true);
            allColliders.RemoveWhere(x => x.gameObject.wasDestroyed == true);
            allRenderers.RemoveWhere(x => x.gameObject.wasDestroyed == true);
        }

        public Action OnUpdate;
        /// <summary>
        /// Called before the next frame is drawn
        /// </summary>
        public virtual void Update()
        {
            ClearNullGameObjects();
            OnUpdate.Invoke();
        }

        public Action OnLateUpdate;
        /// <summary>
        /// Called after the next frame is drawn
        /// </summary>
        public virtual void LateUpdate()
        {
            ClearNullGameObjects();
            OnLateUpdate.Invoke();

            if (Input.pressedOnThisFrame)
                Input.ClearPressedKeys();
            if (Input.releasedOnThisFrame)
                   Input.ClearReleasedKeys();

            Input.pressedOnThisFrame = true;
            Input.releasedOnThisFrame = true;
        }

        public Action OnFixedUpdate;
        /// <summary>
        /// Called at a fixed rate; Use for physics calculation
        /// </summary>
        public virtual void FixedUpdate()
        {
            ClearNullGameObjects();
            OnFixedUpdate.Invoke();
        }
    }
}
