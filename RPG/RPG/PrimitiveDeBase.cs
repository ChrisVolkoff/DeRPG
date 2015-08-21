using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   public abstract class PrimitiveDeBase : Microsoft.Xna.Framework.DrawableGameComponent
   {
      public RPG Jeu { get; protected set; }
      protected int NbSommets { get; set; }
      protected int NbTriangles { get; set; }
      protected Matrix Monde { get; set; }

      protected PrimitiveDeBase(RPG jeu)
         :base(jeu)
      {
         Jeu = jeu;
         Monde = Matrix.Identity;
      }

      public override void Initialize()
      {
         InitialiserSommets();
         base.Initialize();
      }
     
      protected  abstract void InitialiserSommets();

      public virtual Matrix GetMonde()
      {
         return Monde;
      }
   }
}