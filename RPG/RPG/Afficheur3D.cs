using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class Afficheur3D : Microsoft.Xna.Framework.DrawableGameComponent
   {
      RPG Jeu { get; set; }
      InputManager GestionInput { get; set; }
      public DepthStencilState JeuDepthBufferState { get; private set; }
      public RasterizerState JeuRasterizerState { get; private set; }
      public BlendState JeuBlendState { get; private set; }


      bool estAffich�EnWireframe_;
      bool EstAffich�EnWireframe
      {
         get { return estAffich�EnWireframe_; }
         set
         {
            JeuRasterizerState = new RasterizerState();
            estAffich�EnWireframe_ = value;
            JeuRasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            if (estAffich�EnWireframe_)
            {
               JeuRasterizerState.FillMode = FillMode.WireFrame;
            }
            else
            {
               JeuRasterizerState.FillMode = FillMode.Solid;
            }
            Jeu.GraphicsDevice.RasterizerState = JeuRasterizerState;
         }
      }

      public Afficheur3D(RPG jeu)
         : base(jeu)
      {
         Jeu = jeu;
      }

      public override void Initialize()
      {
         JeuDepthBufferState = new DepthStencilState();
         JeuDepthBufferState.DepthBufferEnable = true;
         JeuRasterizerState = new RasterizerState();
         JeuRasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
         JeuBlendState = BlendState.Opaque;
         base.Initialize();
      }

      protected override void LoadContent()
      {
         GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         GestionClavier();
         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         GraphicsDevice.DepthStencilState = JeuDepthBufferState;
         GraphicsDevice.RasterizerState = JeuRasterizerState;
         GraphicsDevice.BlendState = JeuBlendState;
         GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp; //LinearWrap
         base.Draw(gameTime);
      }

      private void GestionClavier()
      {
         if (GestionInput.EstNouvelleTouche(Keys.F))
         {
            EstAffich�EnWireframe = !EstAffich�EnWireframe;
         }
      }
   }
}