   M S B A M L   `   `   `  ����C  ?UAV_Info, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:UAV_InfoUAV_Info  5   	   +clr-namespace:UAV_Info.MapUAV_Info.Map  5   	       UAV_Info.Window5   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:UAV_Info     5   	   b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   $mapclr-namespace:UAV_Info.Map  5   	   -   5          � Title$  Window2��5
   	   $
��1080��6   $
��1920��6'    � WindowStyle$
 None=�64    � AllowsTransparency .  � WindowState$ 	Maximized=�6a   ���	#FF3A50B7.��5      (�  (� Stretch$
 Fill=�6   .��5   
   � $
��1080��6   $
��1920��6    � RowDefinitions 5      �� 5      $��50 �6    �� 5      .+�5      � $��50��6   ���	#FF001680$��0,0,0,0q�66   $	��Top=�6G   $��Center=�6_   $
��1920��6|   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Right=�66   $��Center=�6R   $
��10,0q�6m   .+�5      �� -   5         *  &MaterialDesignFloatingActionMiniButton#��[  5      $��30��6]   $��30��6i   ��5      Z� ��63   Z� .��5      �� $	����6$   $��
/#iconfont)�64   ���#FFFFFF$��18(�6a   $��0,0,0,0q�6o   5      �� 5      -   5        #��[  5      $��30��6]   $��30��6i   $��30,0,0,0q�5   J   ��6   Z� ��63   Z� .��5      �� $	����6$   $��
/#iconfont)�64   ���#FFFFFF$��18(�6a   $��0,0,0,0q�6o   5      �� 5      -   5        #��[  5      $��30��6]   $��30��6i   $��30,0,0,0q�5    J   ��6   Z� ��63   Z� .��5!      �� $	����6$   $��
/#iconfont)�64   ���#FFFFFF$��18(�6a   $��0,0,0,0q�6o   5"      5#      5$      � 5'      $	��Top=�6   $��0,0,0,0q�6+   $��1��6<   $
��1920��6I   $
��1030��6V    � ColumnDefinitions 5)      �� 5*      $	��450 �6'   �� 5+      .+�50      �� ���#FFFFFF K� ZIndex$ 1��6.   	 J� Effect	 51      3  -System.Windows.Media.Effects.DropShadowEffect  52      
   Color$
 Black��6+      
BlurRadius$ 30Q�69      ShadowDepth$ 20Q�6I      Opacity$	 0.1Q�6Z   54      � 58      $ 1��6   ��59      �� 5:       �� ImageSource$ background.jpg��6%    j� Stretch$ UniformToFill=�6B    �� Opacity$	 0.2Q�6Z    5=      �� 5>      $	��120 �6(   �� 5?      .+�5C      �� $��UAV Info��6    $��Center=�60   $��Center=�6M   $��40(�6h   $��Medium"�6v   �� 5E      $��	50 0 50 0q�6   
�������$��0 0 0 1q�6G   �� 5G      $��1��6!   .+�5H      �� -   5         ��Name$ 	W1_Button��5H   !   $��
15 30 15 0q�64   $��90��6H   $	��380��6T   $
��Left=�6`   +  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth3=�5I   !   
������ $��0q�6k   $��Consolas)�5J   9   .��5K      �� $ 
Horizontal=�6)   $	��250��6B   .+�5L   "   �� $	����6,   $��
/#iconfont)�6<   $��38(�6T   ���#848484$��10 0 0 0q�6w   �� 5M   "   $��Position��6,   $��Center=�6<   ���#848484$��30 0 0 0q�6l   $��22(�6~   5N      5O      �� 5P      -   5        $ 	W2_Button��5P   !   $��
15 30 15 0q�64   $��90��6H   $	��380��6T   $
��Left=�6`   $ Depth3=�5Q   !   
������ $��0q�6k   $��Consolas)�5R   9   .��5S      �� $ 
Horizontal=�6)   $	��250��6B   .+�5T   "   �� $	����6,   $��
/#iconfont)�6<   ���#848484$��38(�6i   $��10 0 0 0q�6w   �� 5U   "   $��
Link State��6,   $��Center=�6>   ���#848484$��30 0 0 0q�6n   $��22(�6�   5V      5W      �� 5X      -   5        $ 	W3_Button��5X   !   $��
15 30 15 0q�64   $��90��6H   $	��380��6T   $
��Left=�6`   $ Depth3=�5Y   !   
������ $��0q�6k   $��Consolas)�5Z   9   .��5[      �� $ 
Horizontal=�6)   $	��250��6B   .+�5\   "   �� $	����6,   $��
/#iconfont)�6<   ���#848484$��38(�6i   $��10 0 0 0q�6w   �� 5]   "   $��Node Setting��6,   $��Center=�6@   ���#848484$��30 0 0 0q�6p   $��22(�6�   5^      5_      �� 5`      -   5        $ 	W4_Button��5`   !   $��
15 30 15 0q�64   $��90��6H   $	��380��6T   $
��Left=�6`   $ Depth3=�5a   !   
������ $��0q�6k   $��Consolas)�5b   9   .��5c      �� $ 
Horizontal=�6)   $	��250��6B   .+�5d   "   �� $	����6,   $��
/#iconfont)�6<   ���#848484$��38(�6i   $��10 0 0 0q�6w   �� 5e   "   $��Posture��6,   $��Center=�6;   ���#848484$��30 0 0 0q�6k   $��22(�6}   5f      5g      �� 5h        MaterialDesignRaisedButton#���  6!   $��
15 30 15 0q�6V   $��90��6j   $	��380��6v   $
��Left=�5i   !   $ Depth3=�6<   ���	#FF3A50B7$��Consolas)�6�   .��5j      �� $ 
Horizontal=�6)   $	��250��6B   .+�5k   "   �� $	����6,   $��
/#iconfont)�6<   $��38(�6U   $��10 0 0 0q�6c   �� 5l   "   $	��Log��6,   $��Center=�67   $��30 0 0 0q�6R   $��22(�6d   5m      5n      5o      5p      � 5t      $��1��6   ���#EEEEEE 5u      �� 5v      $��3* �6(   �� 5w      $��7* �6(   .+�5y      � $��0��6    5z      �� 5{      $��6* �6/   �� 5|      $��4* �6/   .+�5~      � $��0��6   .+�5      &   System.Windows.Controls.DataGrid  -	   5        $��
mydatagrid��5   (      AutoGenerateColumns .  "   VerticalScrollBarVisibility$ Visible=�6Z   $	��840��6�   $��20q�5�   (      SelectionMode$ Extended=�6L      SelectionUnit$ FullRow=�6e      
IsReadOnly . $��20(�5�   :   $��Consolas)�6H   ��5�   4   ��    Columns 5�   "   0  *System.Windows.Controls.DataGridTextColumn  5�   &   ,  &System.Windows.Controls.DataGridColumn   Header$	 UAV��69      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6F   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6R   �� *	Devname+6p     5�   &   $ TaskId��69   $	 250 6I    6U   �� *TaskId+6r     5�   &   $ FlyTime��69   $	 240 6J    6V   �� *	FlyTime+6t     5�   &   $ LandTime��69   $	 240 6K    6W   �� *
LandTime+6v   5�      5�      � 5�      $��1��6    5�      �� 5�   "   $��3* �60   �� 5�   "   $��2* �60   .+�5�      � $��0��6#   $��Center=�60   $��0 50 0 0q�6M   .+�5�   "   �� .+�5�   &   �  5�   *   �� 5�   .   �� 5�   .   5�   '   �� 5�   &   -
   5         ��Name$ FilledComboBox3��5�   /      MaterialDesignFilledComboBox#��[ 5�   )    �� DisplayMemberPath$ TaskId��5�   )    �� SelectedValuePath$ TaskId��6D   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ TaskId��5�   )   $��20(�6Q   $	��400��6_   $��Consolas)�6�   {�5�   )   ��   �� Path$  	IsChecked�! �� ElementName$$! FilledComboBoxEnabledCheckBox3��5�   '   5�   #   5�      � 5�      $��1��6#   $��Center=�60   $��Bottom=�6M   �� 5�      -   5        $ 
Show_Again��5�   %    " MaterialDesignRaisedDarkButton#��[ 69   $��50��5�   !   $	��120��6-   +	  %MaterialDesignThemes.Wpf.ButtonAssist" 	  CornerRadius$" 25��69   # � ToolTip$'# !Click Here To Show The Task Again��5�   !   $��94,0.4,374,72.8q�6h   $��1��6�   $��Consolas)�6�   .��5�   "   �� $��
Show Again��6-   5�      �� 5�      -   5        $ 	delete_bt��5�   %   #��[ 68   $��50��5�      $	��120��6%   $" 25��61   $# Delete the log��5�      $��235,0.4,233,72.8q�6I   $��1��6c   $��Consolas)�6p   .��5�   "   �� $��Delete��6-   5�      �� 5�      -   5        $ delete_all_bt��5�   %   #��[ 6<   $��50��5�   !   $	��120��6-   $" 25��69   $# Delete the log��5�   !   $��374,0.4,94,72.8q�6U   $��1��6n   .��5�   "   �� $��
Delete All��6-   $��Consolas)�6?   5�      5�      5�      � 5�      $��1��6    5�      �� 5�      $��1* �6/   �� 5�      $��1* �6/   .+�5�      � $��0��6   .+�5�      �� 
�������$��1,1,1,1q�6<   $	��620��6V   $	��620��6b   $��Center=�5�   %   $��Center=�6B   ._�5�   "   
  UAV_Info.Map.MapControl
  -   5        $ 
 Name$$ mapControl2��5�   1   % �� BitmapScalingMode$% NearestNeighbor=�6F   & � UseLayoutRounding& . ' J� SnapsToDevicePixels' . c _GMap.NET.WindowsPresentation, Version=2.0.2.0, Culture=neutral, PublicKeyToken=b85b9027b614afef.  (GMap.NET.WindowsPresentation.GMapControl(   MinZoom$( 10��5�   g   )   MaxZoom$) 18��6t   *   Zoom$* 12Q�6�   5�      5�      � 5�      $��1��6   .+�5�      �� 
�������$��1,1,1,1q�6<   $	��620��6V   $	��620��6b   $��Center=�6o   $��Center=�6�   ._�5�   "      UAV_Info.HUD  -   5        +  Name$
+ hud2��5�   ,   $	��620��6:   $	��620��6F   $��Center=�6S   $��Center=�6n   ,   MaxRollAngle$, 80Q�6�   $��Consolas)�6�   5�      5�      5�      5�      5�      5�      5�      5�      