using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Rollerghoster.Windows {
    public class Ghost : SyncScript {
        public List<Vector3> positions { get; private set; }

        public void SetPositions(List<Vector3> positions) {
            this.positions = positions;
        }

        public override void Update() {
 
        }
    }
}
