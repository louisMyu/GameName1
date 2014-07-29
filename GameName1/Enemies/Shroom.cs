﻿using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Shroom : GameObject, IEnemy
    {
        private AnimationTimer m_BlinkingTimer;
        float[] m_BlinkingIntervals = new float[2];
        string[] m_BlinkingTextures = new string[2];
        const string BlinkAnimationName = "BlinkingAnimation";
        private const int DAMAGE_AMOUNT = 5;
        public enum MotionState
        {
            Wandering,
            Locked,
            Dead
        }
        [DataMember]
        public Vector2 bodyPosition { get; set; }
        [IgnoreDataMember]
        static private Texture2D m_Texture = null;
        [IgnoreDataMember]
        private float m_Speed = 2.5f;
        [DataMember]
        public float Speed { get { return m_Speed; } set { m_Speed = value; } }

        [IgnoreDataMember]
        public Body _circleBody;

        [DataMember]
        public int LifeTotal { get; set; }

        [DataMember]
        public MotionState State { get; set; }
        public Shroom()
            : base()
        {
            LifeTotal = 40;
            m_BlinkingIntervals[0] = 2500;
            m_BlinkingIntervals[1] = 1000;
            m_BlinkingTextures[0] = "ShroomEyeClosed";
            m_BlinkingTextures[1] = "ShroomEyeOpen";
            m_BlinkingTimer = new AnimationTimer(m_BlinkingIntervals, BlinkAnimationName, HandleAnimation, true);
            circleRadius = 65;
        }
        private void HandleAnimation(object o, AnimationTimerEventArgs e)
        {
            switch (e.AnimationName)
            {
                case BlinkAnimationName:
                    m_Texture = TextureBank.GetTexture(m_BlinkingTextures[e.FrameIndex]);
                    break;
            }
        }

        public void LoadContent(World world)
        {
            m_Direction = new Vector2(0, 0);
            foreach (string s in m_BlinkingTextures)
            {
                TextureBank.GetTexture(s);
            }
            m_Texture = TextureBank.GetTexture(m_BlinkingTextures[0]);

            Width = m_Texture != null ? m_Texture.Width : 0;
            Height = m_Texture != null ? m_Texture.Height : 0;

            if (m_Texture != null)
            {
                m_Bounds.Width = Width;
                m_Bounds.Height = Height;
                m_Bounds.X = (int)Position.X - Width / 2;
                m_Bounds.Y = (int)Position.Y - Height / 2;
                m_Origin.X = Width / 2;
                m_Origin.Y = Height / 2;
            }

            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 5f;
            _circleBody.LinearDamping = 3f;
            _circleBody.Restitution = .5f;
            

            circleCenter = Position;
            circleCenter.Y += circleRadius;
        }

        private Vector2 circleCenter;
        private float circleRadius;
        bool backAndForth = true;
        float moveTime;
        float circleTime = 0;
        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc, TimeSpan elapsedTime)
        {
            circleTime += (float)elapsedTime.TotalMilliseconds;
            //should really just use the Sim's position for everything instead of converting from one to another
            Vector2 simPosition = ConvertUnits.ToDisplayUnits(_circleBody.Position);
            if (float.IsNaN(simPosition.X) || float.IsNaN(simPosition.Y))
            {
                return;
            }
            else
            {
                this.Position = simPosition;
            }
            moveTime += (float)elapsedTime.TotalSeconds;
            if (backAndForth)
            {
                circleCenter.X += 1;
            }
            else
            {
                circleCenter.X -= 1;
            }
            if (moveTime > 5)
            {
                backAndForth = !backAndForth;
                moveTime = 0;
            }
            float speedScale = (float)(0.001 * 2 * Math.PI) / Speed;
            float angle = circleTime * speedScale;
            if (angle > Math.PI * 2) circleTime = 0;
            Vector2 newPos = new Vector2();
            newPos.X = circleCenter.X + (float)Math.Sin(angle) * circleRadius;
            newPos.Y = circleCenter.Y + (float)Math.Cos(angle) * circleRadius;
            Position = newPos;
            RotationAngle = (float)Math.PI / 2;
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }

            m_Bounds.X = (int)Position.X - Width / 2;
            m_Bounds.Y = (int)Position.Y - Height / 2;
        }
        
        public void DoPuff()
        {

        }
        public override void Update(Player player, TimeSpan elapsedTime)
        {
            ObjectManager.GetCell(Position).Remove(this);
            Move(player.Position, elapsedTime);
            ObjectManager.GetCell(Position).Add(this);

            bodyPosition = _circleBody.Position;
            m_BlinkingTimer.Update(elapsedTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, RotationAngle, m_Origin, 1.0f, SpriteEffects.None, 0f);
        }

        public static void LoadTextures()
        {
            
        }
        #region IEnemy
        public void CleanBody()
        {
            if (_circleBody != null)
            {
                GameplayScreen.m_World.RemoveBody(_circleBody);
            }
        }
        public void ApplyLinearForce(Vector2 angle, float amount)
        {
            Vector2 impulse = Vector2.Normalize(angle) * amount;
            _circleBody.ApplyLinearImpulse(impulse);
        }
        public void AddToHealth(int amount)
        {
            LifeTotal += amount;
        }
        public List<Texture2D> GetExplodedParts()
        {
            return ExplodedParts;
        }
        protected override void LoadExplodedParts()
        {
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart1"));
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart2"));
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart3"));
        }
        public int GetHealth()
        {
            return LifeTotal;
        }
        public int GetDamageAmount()
        {
            return DAMAGE_AMOUNT;
        }
        public void DoCollision(Player player)
        {
            player.LifeTotal -= GetDamageAmount();
            ObjectManager.RemoveObject(this);
        }
        #endregion
        #region Save/Load
        public override void Save()
        {
        }
        public override void Load(World world)
        {
            if (m_Texture == null)
            {
                m_Texture = TextureBank.GetTexture("ShroomTexture");
            }
            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 0.2f;
            _circleBody.LinearDamping = 2f;
            _circleBody.Position = bodyPosition;
        }
        #endregion
        private class ShrromExplosion
        {
            private float AnimationTime;
        }
    }
}
