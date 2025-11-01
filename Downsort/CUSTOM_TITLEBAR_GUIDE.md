# 커스텀 타이틀바 적용 가이드

## MainWindow.xaml 수정 방법

### 1. ThemedWindow 속성 변경

```xaml
<dx:ThemedWindow
    <!-- ... 기존 속성 ... -->
    ShowTitle="False"
    WindowStyle="None"
    ResizeMode="CanResizeWithGrip">
```

### 2. Resources에 추가할 아이콘

```xaml
<dx:ThemedWindow.Resources>
    <!-- 기존 아이콘들 ... -->
    
    <!-- Window Control Icons -->
    <PathGeometry x:Key="MinimizeIcon">M19,13H5V11H19V13Z</PathGeometry>
    <PathGeometry x:Key="MaximizeIcon">M4,4H20V20H4V4M6,8V18H18V8H6Z</PathGeometry>
    <PathGeometry x:Key="RestoreIcon">M4,8H8V4H20V16H16V20H4V8M16,8V14H18V6H10V8H16M6,12V18H14V12H6Z</PathGeometry>
    <PathGeometry x:Key="CloseIcon">M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z</PathGeometry>
    
    <!-- Title Bar Button Style -->
    <Style x:Key="TitleBarButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="Transparent"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="Width" Value="46"/>
        <Setter Property="Height" Value="32"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Background="{TemplateBinding Background}">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
        <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter Property="Background" Value="#0D47A1"/>
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter Property="Background" Value="#01579B"/>
            </Trigger>
        </Style.Triggers>
    </Style>
    
    <Style x:Key="TitleBarCloseButtonStyle" TargetType="Button" BasedOn="{StaticResource TitleBarButtonStyle}">
        <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter Property="Background" Value="#E81123"/>
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter Property="Background" Value="#C1091A"/>
            </Trigger>
        </Style.Triggers>
    </Style>
</dx:ThemedWindow.Resources>
```

### 3. Grid 구조 변경

기존:
```xaml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- Toolbar -->
        <RowDefinition Height="*"/>     <!-- Main Content -->
        <RowDefinition Height="Auto"/>  <!-- Status Bar -->
    </Grid.RowDefinitions>
```

변경:
```xaml
<Border BorderBrush="#1976D2" BorderThickness="1">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- Custom Title Bar -->
            <RowDefinition Height="Auto"/>  <!-- Toolbar -->
            <RowDefinition Height="*"/>     <!-- Main Content -->
            <RowDefinition Height="Auto"/>  <!-- Status Bar -->
        </Grid.RowDefinitions>
```

### 4. 커스텀 타이틀바 추가 (Grid.Row="0")

```xaml
<!-- Custom Title Bar -->
<Border Grid.Row="0" Background="#1976D2" MouseLeftButtonDown="TitleBar_MouseLeftButtonDown">
    <Grid Height="32">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>
        
        <!-- App Icon and Title -->
        <StackPanel Grid.Column="0" Orientation="Horizontal" Margin="10,0,0,0" VerticalAlignment="Center">
            <Path Data="M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2M12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4M12,6A6,6 0 0,1 18,12A6,6 0 0,1 12,18A6,6 0 0,1 6,12A6,6 0 0,1 12,6M12,8A4,4 0 0,0 8,12A4,4 0 0,0 12,16A4,4 0 0,0 16,12A4,4 0 0,0 12,8Z"
                  Fill="White" 
                  Width="16" 
                  Height="16" 
                  Stretch="Uniform"
                  Margin="0,0,8,0"/>
            <TextBlock Text="DownSort" 
                       Foreground="White" 
                       FontSize="13" 
                       FontWeight="SemiBold" 
                       VerticalAlignment="Center"/>
            <TextBlock Text="Smart File Organizer" 
                       Foreground="#B3E5FC" 
                       FontSize="11" 
                       Margin="8,0,0,0" 
                       VerticalAlignment="Center"/>
        </StackPanel>
        
        <!-- Window Controls -->
        <StackPanel Grid.Column="2" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Click="MinimizeButton_Click" 
                    Style="{StaticResource TitleBarButtonStyle}"
                    ToolTip="Minimize">
                <Path Data="{StaticResource MinimizeIcon}" 
                      Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}"
                      Width="10" 
                      Height="10" 
                      Stretch="Uniform"/>
            </Button>
            
            <Button Click="MaximizeRestoreButton_Click" 
                    x:Name="MaximizeRestoreButton"
                    Style="{StaticResource TitleBarButtonStyle}"
                    ToolTip="Maximize">
                <Path x:Name="MaximizeRestoreIcon"
                      Data="{StaticResource MaximizeIcon}" 
                      Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}"
                      Width="10" 
                      Height="10" 
                      Stretch="Uniform"/>
            </Button>
            
            <Button Click="CloseButton_Click" 
                    Style="{StaticResource TitleBarCloseButtonStyle}"
                    ToolTip="Close">
                <Path Data="{StaticResource CloseIcon}" 
                      Fill="{Binding Foreground, RelativeSource={RelativeSource AncestorType=Button}}"
                      Width="10" 
                      Height="10" 
                      Stretch="Uniform"/>
            </Button>
        </StackPanel>
    </Grid>
</Border>
```

### 5. 기존 행 번호 업데이트

- Toolbar: `Grid.Row="0"` → `Grid.Row="1"`
- Main Content: `Grid.Row="1"` → `Grid.Row="2"`
- Status Bar: `Grid.Row="2"` → `Grid.Row="3"`

### 6. Border 닫기 태그 추가

맨 끝에 `</Border>` 추가

---

## 완성된 모습

```
┌────────────────────────────────────────────────────┐
│ [앱아이콘] DownSort  Smart File Organizer  [_][□][X] │ ← 커스텀 타이틀바
├────────────────────────────────────────────────────┤
│ [Watch] [Scan] [Execute] [Undo] [Clear]  [Status] │ ← 툴바
├────────────────────────────────────────────────────┤
│                                                     │
│                 Main Content                        │
│                                                     │
├────────────────────────────────────────────────────┤
│ ● Ready                        Monitoring: ON      │ ← 상태바
└────────────────────────────────────────────────────┘
```

---

## 색상 커스터마이징

### Material Blue (현재 설정)
- 타이틀바: `#1976D2`
- Hover: `#0D47A1`
- Active: `#01579B`

### 다크 모드
```xaml
<Border Grid.Row="0" Background="#263238" ...>
    <!-- Hover: #37474F -->
    <!-- Active: #455A64 -->
</Border>
```

### Accent Color
```xaml
<Border Grid.Row="0" Background="#00BCD4" ...>
    <!-- Cyan -->
</Border>
```

---

## 문제 해결

### 창 테두리가 보이지 않음
- `AllowsTransparency="False"` 설정 확인
- `WindowStyle="None"` 설정 확인

### 창을 드래그할 수 없음
- `MouseLeftButtonDown="TitleBar_MouseLeftButtonDown"` 확인
- Code-behind에 이벤트 핸들러 추가 확인

### 최대화/복원 버튼이 작동하지 않음
- `StateChanged` 이벤트 핸들러 확인
- `x:Name="MaximizeRestoreButton"` 및 `x:Name="MaximizeRestoreIcon"` 확인

---

**Code-behind (MainWindow.xaml.cs)는 이미 업데이트되었습니다!**

이제 MainWindow.xaml 파일을 위 가이드대로 수정하면 완성입니다.
