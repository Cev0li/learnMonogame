﻿#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using cevEngine2D.source.world.units;
#endregion

namespace cevEngine2D.source.engine {
    internal class BasicUnit {
        internal Texture2D _texture;
        protected Vector2 _pos, _size;
        protected Rectangle _dRect, _sRect, _hitbox;
        //Properties
        public Texture2D SpriteTexture { get { return _texture; } }
        public Vector2 POS { get { return _pos; } }
        public Vector2 Size { get { return _size; } }
        public Rectangle DRect { get { return _dRect; } set { _dRect = value; } }
        public Rectangle SRect { get { return _sRect; } set { _sRect = value; } }
        public Rectangle Hitbox { get { return _hitbox;  } set { _hitbox = value; } }


        public BasicUnit(string texture, Vector2 pos, Vector2 size, Rectangle sRect) {
            _texture = Globals.content.Load<Texture2D>(texture);
            _pos = pos;
            _size = size;

            _dRect = new Rectangle(
                (int)(pos.X - size.X / 2), 
                (int)(pos.Y - size.Y / 2), 
                (int)size.X, 
                (int)size.Y
            );
            _sRect = sRect;

            _hitbox = GetNonTransparentBounds(SpriteTexture, SRect);
        }

        public Rectangle GetNonTransparentBounds(Texture2D texture, Rectangle sourceRectangle) {
            Vector2 hitboxScale = _size / new Vector2(_sRect.Width, _sRect.Height); 
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            int left = sourceRectangle.Right;
            int right = sourceRectangle.Left;
            int top = sourceRectangle.Bottom;
            int bottom = sourceRectangle.Top;

            for (int y = sourceRectangle.Top; y < sourceRectangle.Bottom; y++) {
                for (int x = sourceRectangle.Left; x < sourceRectangle.Right; x++) {
                    Color pixel = data[y * texture.Width + x];
                    if (pixel.A != 0) // If pixel is not transparent
                    {
                        left = Math.Min(left, x);
                        right = Math.Max(right, x);
                        top = Math.Min(top, y);
                        bottom = Math.Max(bottom, y);
                    }
                }
            }

            return new Rectangle(
                left, 
                top, 
                (int)(Math.Round((right - left + 1) * hitboxScale.X)), 
                (int)(Math.Round((bottom - top + 1) * hitboxScale.Y)));
        }

        public virtual void Update(Vector2 offset) {
            _dRect = new Rectangle(
                (int)(_pos.X - _size.X / 2),
                (int)(_pos.Y - _size.Y / 2),
                (int)_size.X,
                (int)_size.Y
            );
            updateHitBox(offset);
        }

        public virtual void Update() { 
            _dRect = new Rectangle(
                (int)(_pos.X - _size.X / 2), 
                (int)(_pos.Y - _size.Y / 2),
                (int)_size.X,
                (int)_size.Y
            );
            updateHitBox(GameGlobals.camera.Position);
        }

        protected void updateHitBox(Vector2 offset) {
            _hitbox = new Rectangle(
                _dRect.X + _dRect.Width / 2 - _hitbox.Width / 2 + (int)offset.X,
                _dRect.Y + _dRect.Height / 2 - _hitbox.Height / 2 + (int)offset.Y,
                _hitbox.Width,
                _hitbox.Height
            );
        }

        public virtual void Draw() {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect = new Rectangle(
                    (int)(_pos.X - _size.X / 2) + (int)GameGlobals.camera.Position.X,
                    (int)(_pos.Y - _size.Y / 2) + (int)GameGlobals.camera.Position.Y,
                    (int)_size.X,
                    (int)_size.Y
                ),
                _sRect,
                Color.White);
        }

        public virtual void Draw(Vector2 offset) {
            Globals.spriteBatch.Draw(
                _texture,
                _dRect = new Rectangle(
                    (int)((_pos.X - _size.X / 2) + offset.X),
                    (int)((_pos.Y - _size.Y / 2) + offset.Y),
                    (int)_size.X,
                    (int)_size.Y
                ),
                _sRect,
                Color.White);
        }
    }
}
