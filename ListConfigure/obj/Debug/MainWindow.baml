   M S B A M L   `   `   `  ����H  DListConfigure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:ListConfigure.modelListConfigure.model  5   	   A%clr-namespace:ListConfigure.converterListConfigure.converter  5   	   -clr-namespace:ListConfigureListConfigure  5   	   T PMahApps.Metro, Version=1.4.3.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls 5	   	       ListConfigure.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3model!clr-namespace:ListConfigure.model     5   	   ;	converter%clr-namespace:ListConfigure.converter     5   	   -localclr-namespace:ListConfigure     5   	   PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5	   	     � Title$  List Configuration Tool��5   	   $	��700��6)   $
��1000��66    � 	Resources 5      7  1ListConfigure.converter.NullToVisibilityConverter   NullToVisibility>  8ListConfigure.converter.BoolToInverseVisibilityConverter  BoolToInverseVisibility1  +ListConfigure.converter.NullToBoolConverter  
NullToBool4  .ListConfigure.converter.StatusToColorConverter  StatusToColor��%t   &	        &	      &	 &     &	 9       5   
   6.     5   
   65     5   
   6(     5   
   6+   5      .��5      � ���#f0f0f0 � RowDefinitions 5   
   �� 5      $��50 �6   �� 5      $��* �6   �� 5      $��5 �6   �� 5      $	��230 �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��5 �6   �� 5      $	��250 �6   .+�5   
   �� -   5         ��Name$ DirSelector��5   .    �� Orientation$ 
Horizontal=�6   $��10q�6C   .+�5!      �� -   5        $��Center=�5!   -   $��0 0 5 0q�6H   $	��140��6Y    � DataContext 5"      '  !ListConfigure.model.Configuration  5#      -   5        MyConfig��5#   .   .��5$   *   Set Source Directory5&      �� 5'      -   5         ��Name$	 Dir��5'      $��Center=�6g   ���#fff���#5d5d5d �� Padding$ 4q�6�   ��6)   �� 	 �� ElementName$	 MyConfig��
 �� Path$
 SelectedDirectory�y�5(      �� $	 MyConfig��$
 SelectedDirectory� �� 	Converter# [  �� 5)      -   5        $��Center=�5)      $��15 4 0 0q�64     
LinkButton#��[ 6[   y�5*      �� $	 MyConfig��$
 SelectedDirectory�# [  .��6�   	Refresh5,      5-      �� 5.      -   5         ��Name$ FileListView��5.      $��0��6j   $��1��6z   $��5 3 0 0q�6�   $��1q�6�   ���#6d6d6d��6-   �� $	 MyConfig��$
 	ListFiles���5/      �� 50      .R�51      �� 
�������$��0 0 0 1q�60    �� Padding$ 6q�6J   ._�52      �  53      �� 54   "   $��* �60    56      �� 57   "   $��* �63   �� 58   "   $	��100 �63   .+�5:      �� $��0��6)   $��0��66   $ 
Horizontal=�6G   .+�5;   "   �� $��0 0 5 0q�6Y    d� 	IsChecked 6+   �� *
IsIncluded+ �� Mode$ TwoWay=�6l   �� 5<   "   .+�5=   &   �� $��17(�6F   $��Medium"�6T   ��60   �� *Name+6T   �� 5>   &   $ 
Horizontal=�61   .+�5?   *   �� $��
Category: ��64   $��13(�6F   
�������6T   �� 5@   *   $��13(�6N   $��Medium"�6\   ���#5d5d5d��64   �� *
Category+6p   5A   '   �� 5B   &   
�������$��11(�6X   ��60   �� *Path+6X   5C   #   5D      �� 5F      $��0��6)   $��1��66   $ 
Horizontal=�6F   .+�5G   "   �� $��Center=�6D   $��Medium"�6�   $��12(�6�   $ 5q�6�   ��6,   �� *Status+��6_   �� *Status+# [ 5H   #   5I      5K      5L      5M      5O      �� 5P   
   $��0��6   $��2��6'   ���#333 �� ResizeDirection$
 Rows=�6F   $��0q�6]   6j   �� 5Q   
   $��0��6   $��3��6!   ���#6d6d6d$��1q�6D   $��5 0 0 20q�6X   ���#fff._�5R      �� -   5         ��Name$ ConsoleScroll��5R   /   $��	10 0 10 0q�6   .��5S      �� .+�5T      �� $��Consolas)�6;   ���#3d3d3d$��11(�6f   $
��Wrap=�6t   ��6    �� $

 Text� 5U      !  ListConfigure.model.Console  5V      -   5        	MyConsole��5V   ,   5X      5Y      5Z      5[      �� 5\   
   $��1��6   $��0��6'   $��4��64   $��5��6E   $��Stretch=�6O   ���#f0f0f06�   �� 5^   
   $��2��6   $��0��6!   $��4��6.   ���#eaeaea._�5_   
   �� -	   5        $ 	RightDock��5_      $��10q�6(   .+�5`      ��  5a        ListConfigure.model.User  5b      -
   5        
MyUser��5b   !   .+�5d      �� $��Medium"�6;   ��6    �� $

 Name�6;   �� 5e      ��6    �� $
 Email��� 5f      ��6    �� $
 Server��� 5g      -   5         ��Name$ SignOutButton��5g      $��0 8 0 0q�6P   .��6a   
Sign out6k   �� 5h      -   5        $ ReloginButton��5h      $��0 8 0 0q�6J   .��6[   
Re-login6e   5j      �� 5k      $��12(�6   $��0 15 0 0q�6"   .��64   Configuration Options6K   	� 5l      -   5         	�Name$ IsCsv��5l   6   $
 TrueY�6   $��2q�6+   .��6E   
Csv file6O   	� 5m      $��2q�6   .��6%   Tab separated text file6>   �� 5n      -   5         ��Name$ IgnoreFirst��5n   "   $��2q�6   .��67   Ignore first row6I   �� 5o      $��
 0 10 0 10q�6   �� 5p      -   5        $ EnableNewCategory��5p   3   $
 TrueY�6   $��2q�6(   .��6N   Enable new category creation6l   �� 5q      -   5        $ EnableReplacing��5q   3   $
 TrueY�6   $��2q�6(   .��6L   " Enable replacing existing values6n   �� 5r      -   5        $	��180��5r      $��20q�6!     
MainButton#��[ 6.   $��0 20 0 0q�6�   {�6R   �� $	 MyConfig��$
 	ListFiles�# [ .��6�   Run Configuration!6�   )  #MahApps.Metro.Controls.ProgressRing  5s      -   5          Name$ MyRing��5s   $     AccentColorBrush#���  64      IsActive .  $��0 10 0 0q�6u   5t      5u      5v      5w      