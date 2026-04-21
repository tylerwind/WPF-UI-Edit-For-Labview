# P&ID Dynamic Designer Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Initialize the WPF Designer and Runtime DLL projects, define the core data contract, and implement a proof-of-concept Pump component with data binding and XML serialization.

**Architecture:** A standalone WPF EXE (Designer) for layout design, and a WPF Class Library (Runtime DLL) for LabVIEW execution, communicating via a flat cluster array (`[ID, Type, Value, State]`).

**Tech Stack:** C# .NET Framework 4.8 (for maximum LabVIEW compatibility), WPF, MSTest.

---

### Task 1: Initialize Solutions and Projects

**Files:**
- Create: `src/PIDDesigner.sln`
- Create: `src/PIDDesigner.App/PIDDesigner.App.csproj`
- Create: `src/PIDDesigner.Runtime/PIDDesigner.Runtime.csproj`
- Create: `tests/PIDDesigner.Tests/PIDDesigner.Tests.csproj`

**Step 1: Create solution and projects**
Run: 
```bash
mkdir src
mkdir tests
cd src
dotnet new sln -n PIDDesigner
dotnet new wpf -n PIDDesigner.App -f net48
dotnet new classlib -n PIDDesigner.Runtime -f net48
cd ../tests
dotnet new mstest -n PIDDesigner.Tests -f net48
cd ../src
dotnet sln PIDDesigner.sln add PIDDesigner.App/PIDDesigner.App.csproj
dotnet sln PIDDesigner.sln add PIDDesigner.Runtime/PIDDesigner.Runtime.csproj
dotnet sln PIDDesigner.sln add ../tests/PIDDesigner.Tests/PIDDesigner.Tests.csproj
dotnet add ../tests/PIDDesigner.Tests/PIDDesigner.Tests.csproj reference PIDDesigner.Runtime/PIDDesigner.Runtime.csproj
```

**Step 2: Verify build**
Run: `dotnet build src/PIDDesigner.sln`
Expected: Build succeeds with 0 errors.

**Step 3: Commit**
```bash
git add src/ tests/
git commit -m "chore: initialize WPF projects and test suite"
```

---

### Task 2: Define Core Data Contract

**Files:**
- Create: `src/PIDDesigner.Runtime/Models/DeviceState.cs`
- Create: `tests/PIDDesigner.Tests/DeviceStateTests.cs`

**Step 1: Write the failing test**
```csharp
// tests/PIDDesigner.Tests/DeviceStateTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Tests
{
    [TestClass]
    public class DeviceStateTests
    {
        [TestMethod]
        public void DeviceState_Initialization_SetsPropertiesCorrectly()
        {
            var state = new DeviceState { ID = "Pump1", Type = 1, Value = 50.5, State = 1 };
            Assert.AreEqual("Pump1", state.ID);
            Assert.AreEqual(1, state.Type);
            Assert.AreEqual(50.5, state.Value);
            Assert.AreEqual(1, state.State);
        }
    }
}
```

**Step 2: Run test to verify it fails**
Run: `dotnet test src/PIDDesigner.sln`
Expected: FAIL (Build error, DeviceState not found).

**Step 3: Write minimal implementation**
```csharp
// src/PIDDesigner.Runtime/Models/DeviceState.cs
namespace PIDDesigner.Runtime.Models
{
    public struct DeviceState
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public double Value { get; set; }
        public int State { get; set; }
    }
}
```

**Step 4: Run test to verify it passes**
Run: `dotnet test src/PIDDesigner.sln`
Expected: PASS.

**Step 5: Commit**
```bash
git add src/PIDDesigner.Runtime/Models/ tests/PIDDesigner.Tests/
git commit -m "feat: define core DeviceState struct for LabVIEW cluster mapping"
```

---

### Task 3: Implement Runtime API Entry Point

**Files:**
- Create: `src/PIDDesigner.Runtime/PIDRuntimeEngine.cs`
- Modify: `tests/PIDDesigner.Tests/PIDRuntimeEngineTests.cs`

**Step 1: Write the failing test**
```csharp
// tests/PIDDesigner.Tests/PIDRuntimeEngineTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIDDesigner.Runtime;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Tests
{
    [TestClass]
    public class PIDRuntimeEngineTests
    {
        [TestMethod]
        public void UpdateDeviceStates_ReceivesArray_UpdatesInternalCache()
        {
            var engine = new PIDRuntimeEngine();
            var states = new[] { new DeviceState { ID = "P1", State = 1 } };
            engine.UpdateDeviceStates(states);
            Assert.AreEqual(1, engine.GetState("P1").State);
        }
    }
}
```

**Step 2: Run test to verify it fails**
Run: `dotnet test src/PIDDesigner.sln`
Expected: FAIL.

**Step 3: Write minimal implementation**
```csharp
// src/PIDDesigner.Runtime/PIDRuntimeEngine.cs
using System.Collections.Generic;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Runtime
{
    public class PIDRuntimeEngine
    {
        private Dictionary<string, DeviceState> _stateCache = new Dictionary<string, DeviceState>();

        public void UpdateDeviceStates(DeviceState[] states)
        {
            if (states == null) return;
            foreach (var state in states)
            {
                _stateCache[state.ID] = state;
                // Future: Raise event to update UI components here
            }
        }

        public DeviceState GetState(string id)
        {
            return _stateCache.ContainsKey(id) ? _stateCache[id] : default(DeviceState);
        }
    }
}
```

**Step 4: Run test to verify it passes**
Run: `dotnet test src/PIDDesigner.sln`
Expected: PASS.

**Step 5: Commit**
```bash
git add src/PIDDesigner.Runtime/PIDRuntimeEngine.cs tests/PIDDesigner.Tests/PIDRuntimeEngineTests.cs
git commit -m "feat: implement Runtime Engine API for updating device states"
```
