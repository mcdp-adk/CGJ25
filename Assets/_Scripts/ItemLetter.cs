using UnityEngine;

namespace _Scripts
{
    public class ItemLetter : ItemBase
    {
        #region 类内方法重写

        protected override void HandleSelfMovement()
        {
            RandomLookRotation(_selfLookRotationMaxAngle);
            Jump();
        }

        #endregion

        #region 类外交互重写

        public override void ItemOnHold()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}