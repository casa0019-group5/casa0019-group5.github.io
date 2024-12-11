using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRotationController : MonoBehaviour
{
    
    [SerializeField] private ARTrackedImageManager trackedImageManager;

   
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        
        foreach (var trackedImage in args.added)
        {
            AdjustRotation(trackedImage);
        }

        
        foreach (var trackedImage in args.updated)
        {
            AdjustRotation(trackedImage);
        }
    }

    
    private void AdjustRotation(ARTrackedImage trackedImage)
    {
        
        trackedImage.transform.localRotation = Quaternion.Euler(0, 0, 0);

       
        Debug.Log($"Tracked Image '{trackedImage.referenceImage.name}' Rotation: {trackedImage.transform.rotation.eulerAngles}");
    }
}
