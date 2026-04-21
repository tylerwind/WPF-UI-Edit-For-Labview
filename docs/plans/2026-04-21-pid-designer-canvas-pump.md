# P&ID Designer Canvas & Pump Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implement the foundational WPF Canvas for component placement and build the first dynamic, data-driven component (Pump) with XAML animations.

**Architecture:** 
- The `IPIDComponent` interface ensures all components share a common data-binding contract.
- The `PumpControl` (WPF UserControl) implements this interface, using XAML `Storyboard` for state-driven rotation animations.
- The `DesignerCanvas` (WPF App) serves as the drop target where components are instantiated.

**Tech Stack:** C# .NET 4.8, WPF (XAML), MSTest.

---

### Task 1: Create Component Interface (IPIDComponent)

**Files:**
- Create: `src/PIDDesigner.Runtime/Controls/IPIDComponent.cs`

**Step 1: Write the interface definition**
```csharp
// src/PIDDesigner.Runtime/Controls/IPIDComponent.cs
namespace PIDDesigner.Runtime.Controls
{
    public interface IPIDComponent
    {
        /// <summary>
        /// Unique identifier mapping to the LabVIEW Cluster array.
        /// </summary>
        string ComponentID { get; set; }

        /// <summary>
        /// Update the visual state and value of the component.
        /// </summary>
        void UpdateData(double value, int state);
    }
}
```

**Step 2: Commit**
```bash
git add src/PIDDesigner.Runtime/Controls/IPIDComponent.cs
git commit -m "feat: define IPIDComponent interface for uniform data binding"
```

---

### Task 2: Implement Pump Control (XAML)

**Files:**
- Create: `src/PIDDesigner.Runtime/Controls/PumpControl.xaml`
- Create: `src/PIDDesigner.Runtime/Controls/PumpControl.xaml.cs`

**Step 1: Write the XAML (Pump UI & Animation)**
```xml
<!-- src/PIDDesigner.Runtime/Controls/PumpControl.xaml -->
<UserControl x:Class="PIDDesigner.Runtime.Controls.PumpControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Width="60" Height="60">
    <UserControl.Resources>
        <Storyboard x:Key="RotateAnimation" RepeatBehavior="Forever">
            <DoubleAnimation Storyboard.TargetName="ImpellerTransform" 
                             Storyboard.TargetProperty="Angle" 
                             From="0" To="360" Duration="0:0:1" />
        </Storyboard>
    </UserControl.Resources>
    <Grid>
        <!-- Outer Casing -->
        <Ellipse Stroke="DarkGray" StrokeThickness="4" Fill="#F0F0F0" />
        <!-- Impeller -->
        <Path Fill="Gray" Data="M30,5 L35,30 L55,30 L35,35 L40,55 L30,40 L20,55 L25,35 L5,30 L25,30 Z" RenderTransformOrigin="0.5,0.5">
            <Path.RenderTransform>
                <RotateTransform x:Name="ImpellerTransform" Angle="0" />
            </Path.RenderTransform>
        </Path>
    </Grid>
</UserControl>
```

**Step 2: Write Code-Behind (IPIDComponent Implementation)**
```csharp
// src/PIDDesigner.Runtime/Controls/PumpControl.xaml.cs
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PIDDesigner.Runtime.Controls
{
    public partial class PumpControl : UserControl, IPIDComponent
    {
        public string ComponentID { get; set; }
        private Storyboard _rotateAnim;

        public PumpControl()
        {
            InitializeComponent();
            _rotateAnim = (Storyboard)this.Resources["RotateAnimation"];
        }

        public void UpdateData(double value, int state)
        {
            // State 1 = Running, 0 = Stopped
            if (state == 1)
            {
                _rotateAnim.Begin(this, true);
            }
            else
            {
                _rotateAnim.Stop(this);
            }
        }
    }
}
```

**Step 3: Commit**
```bash
git add src/PIDDesigner.Runtime/Controls/PumpControl*
git commit -m "feat: implement Pump component with XAML state animations"
```

---

### Task 3: Setup Designer Canvas Skeleton

**Files:**
- Create: `src/PIDDesigner.App/DesignerWindow.xaml`
- Create: `src/PIDDesigner.App/DesignerWindow.xaml.cs`

**Step 1: Write the Designer Window XAML**
```xml
<!-- src/PIDDesigner.App/DesignerWindow.xaml -->
<Window x:Class="PIDDesigner.App.DesignerWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="PID Designer" Height="600" Width="800">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        
        <!-- Toolbox (Left) -->
        <Border BorderBrush="Gray" BorderThickness="0,0,1,0" Background="#FAFAFA">
            <StackPanel Margin="10">
                <TextBlock Text="Toolbox" FontWeight="Bold" Margin="0,0,0,10"/>
                <Button Content="Add Pump" x:Name="BtnAddPump" Click="BtnAddPump_Click" Height="30" />
            </StackPanel>
        </Border>
        
        <!-- Drawing Canvas (Right) -->
        <ScrollViewer Grid.Column="1" HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
            <Canvas x:Name="MainCanvas" Background="White" Width="2000" Height="2000">
                <!-- Components will be dynamically added here -->
            </Canvas>
        </ScrollViewer>
    </Grid>
</Window>
```

**Step 2: Write Designer Window Code-Behind**
```csharp
// src/PIDDesigner.App/DesignerWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using PIDDesigner.Runtime.Controls;

namespace PIDDesigner.App
{
    public partial class DesignerWindow : Window
    {
        public DesignerWindow()
        {
            InitializeComponent();
        }

        private void BtnAddPump_Click(object sender, RoutedEventArgs e)
        {
            var pump = new PumpControl();
            pump.ComponentID = "Pump_" + MainCanvas.Children.Count;
            
            Canvas.SetLeft(pump, 100);
            Canvas.SetTop(pump, 100);
            
            MainCanvas.Children.Add(pump);
        }
    }
}
```

**Step 3: Commit**
```bash
git add src/PIDDesigner.App/DesignerWindow*
git commit -m "feat: setup basic Designer window with Toolbox and Canvas"
```
