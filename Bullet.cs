using BulletHell.Character;
using UnityEngine;

namespace BulletHell.Spawners
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 _dirVector = Vector3.zero;
        // Start is called before the first frame update

        private bool _hasCollided = false;

        private Transform _transform;
        public  float     m_Acceleration;
        
        public float m_Curve;

        public float m_Damage;

        public float m_Direction;

        public float m_DirX;

        public float m_DirY;

        public float m_Speed;

        public float m_Ttl, m_Timer;


        public void Launch(float x, float z, float direction, float speed, float acceleration, float curve, float ttl)
        {
            var transform1        = transform;
            var transformPosition = transform1.position;
            transformPosition.x = x;
            transformPosition.y = z;
            transform1.position = transformPosition;
            m_Direction         = direction;
            m_Speed             = speed;
            m_Acceleration      = acceleration;
            m_Curve             = curve;
            m_Ttl               = ttl;
            m_Timer             = 0f;
        }

        private void Start() { }

        protected virtual void OnCollisionEnter(Collision other)
        {
            Hit(other.collider);
            DestroyBullet();
        }


        protected virtual void Hit(Collider other)
        {
            var otherComponent = other.GetComponent<CharacterCombatComponent>();
            if (otherComponent) DoDamage(otherComponent, m_Color, m_Damage);
        }

        protected virtual void DestroyBullet()
        {
            PoolManager.ReleaseObject(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            RunProjectile();

            // check if outside of screen

            // destroy after timeout
        }


        private void RunProjectile()
        {
            if (m_Timer >= m_Ttl) DestroyBullet();

            m_Direction += m_Curve        * Time.deltaTime;
            m_Speed     += m_Acceleration * Time.deltaTime;

            m_DirX = XDir(m_Direction);
            m_DirY = YDir(m_Direction);

            _dirVector.x = m_DirX;
            _dirVector.y = m_DirY;

            var transform1 = transform;
            transform1.position += _dirVector * (m_Speed * Time.deltaTime);
            m_Timer             += Time.deltaTime;
        }

        public void OnMove()
        {
            ;
        }

        private static float XDir(float angle)
        {
            var rad = Mathf.PI * angle / 180f;
            return Mathf.Cos(rad);
        }

        private static float YDir(float angle)
        {
            var rad = Mathf.PI * angle / 180f;
            return -1f * Mathf.Sin(rad);
        }
    }
}