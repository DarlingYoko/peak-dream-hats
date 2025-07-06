using UnityEngine;

namespace MoreCustomizations.Data {
    
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Hat",
        fileName = "New Custom Hat",
        order    = int.MinValue
    )]
    public class CustomHat_V1 : CustomHatData {
        
        [field: SerializeField]
        public Texture Icon { get; internal set; }
        
        [field: SerializeField]
        public GameObject Prefab { get; internal set; }
        
        [field: SerializeField]
        public Texture MainTexture { get; internal set; }
        
        [field: SerializeField]
        public Texture SubTexture { get; internal set; }
        
        [field: SerializeField]
        public Vector3 PositionOffset { get; internal set; }
        
        [field: SerializeField]
        public Vector3 EulerAngleOffset { get; internal set; }
    }
}