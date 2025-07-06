# Migration strategy

Some properties or data might change in the future.

To be able to proactively respond to this change and maintain backward compatibility,
I decided to utilize the `ScriptableObject` itself as a snapshot of the schema.

## Creating a new schema

1. Create a directory named V[number] in the `Data` directory with a numbered name.
   - If there's latest numbered directory is `V3`, you should create `V4`.
2. Copy the data class whose structure you want to change,
   and do the same for the V[number] in the class name.
3. Create `Migrate` static method like below example.
```csharp
public static CustomAccessoryData_V4 Migrate(CustomAccessoryData_V3 from) {
    
    var result = CreateInstance<CustomAccessoryData_V4>();
    
    //TODO: Migration logics from V3 to V4...
    
    return result;
}
```
4. Deprecate old version of class like below example.
```diff
  using UnityEngine;
  
  namespace MoreCustomizations.Data {
      
-     [CreateAssetMenu(
-         menuName = "PEAK More Customizations/Accessory",
-         fileName = "New Custom Accessory",
-         order    = int.MinValue
-     )]
+     //[CreateAssetMenu(
+     //    menuName = "PEAK More Customizations/Accessory",
+     //    fileName = "New Custom Accessory",
+     //    order    = int.MinValue
+     //)]
+     [System.Obsolete("Use " + nameof(CustomAccessoryData_V2) + " instead.")]
      public class CustomAccessoryData_V1 : ScriptableObject {
          ...
      }
  }
```