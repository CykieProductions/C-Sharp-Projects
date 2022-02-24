using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyTools;

namespace CysmicEngine.Demo_Game
{
    class InputController : Component
    {
        public Vector2 movement = Vector2.Zero;
        public bool pressedJump;
        public float jumpBuffer = 0.2f;
        public float jumpTimer = 0f;

        protected override void Start()
        {
            base.Start();
        }
        protected override void Update()
        {
            base.Update();
            movement = (Input.GetAxis(AxisName.HORIZONTAL), Input.GetAxis(AxisName.VERTICAL));

            if(pressedJump && jumpTimer < jumpBuffer)
            {
                jumpTimer += Time.deltaTime;
            }
            else if (pressedJump)
            {
                jumpTimer = 0;
                pressedJump = false;
            }
            if(Input.PressedKey(Keys.Space))
            {
                pressedJump = true;
                jumpTimer = 0;
            }
        }
    }
    class PlayerMotor : Component
    {
        public InputController ic;
        public Rigidbody2D rb;
        public float speed = 10f;

        public float jumpForce = 6;
        public float jumpHoldLimit = 0.4f;
        float jumpHoldTimer = 0;
        private bool isGrounded = false;

        public Collider2D groundDetector;
        private bool isJumping;

        protected override void Start()
        {
            base.Start();
            TryGetComponent(out ic);
            TryGetComponent(out rb);

            groundDetector = gameObject.AddComponent(new Collider2D((3, 28), (26, 4), true));
        }
        protected override void Update()
        {
            base.Update();
            float moddedSpeed = speed;
            if (Input.HoldingKey(Keys.ShiftKey))
                moddedSpeed = speed * 2;

            isGrounded = groundDetector.IsTouching("Ground");

            if (rb != null)
            {
                //_realXVel = Lerp(_realXVel, velocity.x, 0.25f);//Smooth move
                rb.velocity = (Lerp(rb.velocity.x, ic.movement.x * moddedSpeed, 0.25f), rb.velocity.y);
            }
            else
                transform.Translate((ic.movement.x * Time.deltaTime * moddedSpeed, ic.movement.y * Time.deltaTime * moddedSpeed));
            //print(rb.velocity.x);

            if (isJumping && (rb.velocity.y <= 0 || jumpHoldTimer >= jumpHoldLimit || !Input.HoldingKey(Keys.Space)))
            {
                jumpHoldTimer = 0;
                rb.velocity = (rb.velocity.x, rb.velocity.y / 2);
                isJumping = false;
            }
            if ((ic.pressedJump && isGrounded) || (Input.HoldingKey(Keys.Space) && isJumping))
            {
                rb.AddForce(0f, jumpForce, true);
                jumpHoldTimer += Time.deltaTime;
                isJumping = true;
            }
        }

        public override void OnTriggerEnter(Collider2D other)
        {
            //print($"[ENTER] {other.gameObject.name}", false);
        }
        public override void OnTriggerStay(Collider2D other)
        {
            //print($"[STAY] {other.gameObject.name}", false);
        }
        public override void OnTriggerExit(Collider2D other)
        {
            //print($"[EXIT] {other.gameObject.name}", false);
        }
    }
}
