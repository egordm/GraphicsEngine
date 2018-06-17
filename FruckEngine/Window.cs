﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace FruckEngine {
    public class Window : GameWindow {
        protected Game Game;
        protected double LastMouseX, LastMouseY, LastMouseScroll;
        protected bool FirstMouse = true;
        protected bool LockMouse = true;

        public Window(int width, int height, string title, Game game)
            : base(width, height, GraphicsMode.Default, title) {
            Game = game;
        }

        protected override void OnLoad(EventArgs e) {
            UpdateViewport();
            Game.Init();
            Game.Clear();
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload(EventArgs e) {
            Game.Destroy();
            Environment.Exit(0); // bypass wait for key on CTRL-F5
        }

        protected override void OnResize(EventArgs e) {
            UpdateViewport();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            UpdateMouse();
            if (!Focused || !LockMouse) return;
            var state = Keyboard.GetState();
            Game.OnKeyboardUpdate(state);
            if (state[Key.Escape]) Exit();
            
            Game.Update(e.Time);
        }

        private void UpdateMouse() {
            var state = Mouse.GetState();

            if (!FirstMouse) {
                Game.OnMouseMove(state.X - LastMouseX, state.Y - LastMouseY);
                Game.OnMouseScroll(state.ScrollWheelValue - LastMouseScroll);
            }

            LastMouseX = state.X;
            LastMouseY = state.Y;
            LastMouseScroll = state.WheelPrecise;

            FirstMouse = false;
            ResetCursorPosition();
        }

        protected void ResetCursorPosition() {
            if (!Focused) return;
            Mouse.SetPosition(
                (ClientRectangle.Left - ClientRectangle.Right) / 2f,
                (ClientRectangle.Top - ClientRectangle.Bottom) / 2f);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            Game.Clear();
            Game.Render();
            SwapBuffers();
        }

        private void UpdateViewport() {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
            Game.Resize(Width, Height);
        }
    }
}