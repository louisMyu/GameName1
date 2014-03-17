﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
<<<<<<< HEAD
using System.Runtime.Serialization;
=======
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014

namespace GameName1
{
    //a texture, how many frames the animation should play for, 
    public class AnimationInfo
    {
        public Texture2D Texture;
        public int NextFrame;
        public AnimationInfo(Texture2D tex, int frame)
        {
            Texture = tex;
            NextFrame = frame;
        }
    }
<<<<<<< HEAD
    public class SpriteInfo 
    {
        [DataMember]
        public Vector2 Position;
        [DataMember]
        public float Rotation;
        [DataMember]
        public int NumberOfBullets;
        [DataMember]
        public float LeftAngle; //left most of a spread shot
        public SpriteInfo(Vector2 pos, float rot, int numBul, float left)
        {
            Position = pos;
            Rotation = rot;
            NumberOfBullets = numBul;
            LeftAngle = left;
        }
    }
    class AnimationManager
    {
        AnimationInfo[] AnimationArray;
        private SpriteInfo m_SpriteInfo;
        public SpriteInfo SpriteInfo { set { m_SpriteInfo = value; m_CurrentFrame = 0; FrameCounter = MaxFrames;} }
=======
    class AnimationManager
    {
        AnimationInfo[] AnimationArray;
        private Weapon.ShotInfo m_ShotInfo;
        public Weapon.ShotInfo ShotInfo { set { m_ShotInfo = value; m_CurrentFrame = 0; } }
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
        public bool Finished { get; set; }
        private int m_CurrentFrame;
        public int CurrentFrame { get { return m_CurrentFrame; } }
        public bool Animating { get; set; }
<<<<<<< HEAD
        public int MaxFrames {get;set;}
        public int FrameCounter {get;set;}
        public AnimationManager(AnimationInfo[] array, SpriteInfo shotInfo, int maxFrames)
        {
            m_SpriteInfo = shotInfo;
            AnimationArray = array;
            Finished = false;
            m_CurrentFrame = 0;
            MaxFrames = maxFrames;
=======

        public AnimationManager(AnimationInfo[] array, Weapon.ShotInfo shotInfo)
        {
            m_ShotInfo = shotInfo;
            AnimationArray = array;
            Finished = false;
            m_CurrentFrame = 0;
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
        }

        public void SetAnimation(AnimationInfo[] array)
        {
            AnimationArray = array;
            m_CurrentFrame = 0;
        }
<<<<<<< HEAD
        public void SetSpriteInfo(SpriteInfo info)
        {
            m_SpriteInfo = info;
        }
        public void DrawAnimationFrame(SpriteBatch _spriteBatch)
        {
            if (FrameCounter > 0)
            {
                Animating = true;
                Finished = false;
                if (FrameCounter == AnimationArray[CurrentFrame].NextFrame)
                {
                    ++m_CurrentFrame;
                }
                _spriteBatch.Draw(AnimationArray[CurrentFrame].Texture, m_SpriteInfo.Position, null, Color.White, m_SpriteInfo.Rotation, new Vector2(0, AnimationArray[CurrentFrame].Texture.Height / 2), 1.0f, SpriteEffects.None, 0f);
                --FrameCounter;
=======
		public void SetShotInfo
        public void DrawAnimationFrame(SpriteBatch _spriteBatch)
        {
            if (m_ShotInfo.NumFrames > 0)
            {
                Animating = true;
                Finished = false;
                if (m_ShotInfo.NumFrames == AnimationArray[CurrentFrame].NextFrame)
                {
                    ++m_CurrentFrame;
                }
                _spriteBatch.Draw(AnimationArray[CurrentFrame].Texture, m_ShotInfo.Position, null, Color.White, m_ShotInfo.Rotation, new Vector2(0, AnimationArray[CurrentFrame].Texture.Height / 2), 1.0f, SpriteEffects.None, 0f);
                //foreach (Line line in m_BulletLines)
                //{
                //    line.Draw(_spriteBatch);
                //}
                //if (m_ShotInfo.NumFrames > 12)
                //{
                //    _spriteBatch.Draw(blast, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                //}
                //else if (m_ShotInfo.NumFrames > 9)
                //{
                //    _spriteBatch.Draw(blast2, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                //}
                //else if (m_ShotInfo.NumFrames > 5)
                //{
                //    _spriteBatch.Draw(blast3, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                //}
                //else if (m_ShotInfo.NumFrames > 0)
                //{
                //    _spriteBatch.Draw(blast4, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                //}
                --m_ShotInfo.NumFrames;
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
            }
            else
            {
                Animating = false;
                Finished = true;
            }
        }

        public bool CanStartAnimating()
        {
            return !Animating && Finished;
        }

        public bool CanDraw()
        {
            return Animating || !Finished;
        }
    }
}