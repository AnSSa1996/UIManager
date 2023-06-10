using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    [System.Serializable] 
    public class ScreenUIPriorityLayerListEntry {
        [SerializeField] 
        [Tooltip("The panel priority type for a given target para-layer")]
        private UIPriority priority;
        [SerializeField] 
        [Tooltip("The GameObject that should house all Panels tagged with this priority")]
        private Transform targetParent;

        public Transform TargetParent {
            get { return targetParent; }
            set { targetParent = value; }
        }

        public UIPriority Priority {
            get { return priority; }
            set { priority = value; }
        }

        public ScreenUIPriorityLayerListEntry(UIPriority prio, Transform parent) {
            priority = prio;
            targetParent = parent;
        }
    }

    [System.Serializable] 
    public class ScreenPriorityLayerList {
        [SerializeField] 
        [Tooltip("A lookup of GameObjects to store panels depending on their Priority. Render priority is set by the hierarchy order of these GameObjects")]
        private List<ScreenUIPriorityLayerListEntry> paraLayers = null;

        private Dictionary<UIPriority, Transform> lookup;

        public Dictionary<UIPriority, Transform> ParaLayerLookup {
            get {
                if (lookup == null || lookup.Count == 0) {
                    CacheLookup();
                }

                return lookup;
            }
        }

        private void CacheLookup() {
            lookup = new Dictionary<UIPriority, Transform>();
            for (int i = 0; i < paraLayers.Count; i++) {
                lookup.Add(paraLayers[i].Priority, paraLayers[i].TargetParent);
            }
        }

        public ScreenPriorityLayerList(List<ScreenUIPriorityLayerListEntry> entries) {
            paraLayers = entries;
        }
    }
}
