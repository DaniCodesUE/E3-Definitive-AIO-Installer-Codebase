   M S B A M L   `   `   `  ����Z  VE3 Definitive Mod Demo Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:E3_Definitive_Mod_Demo_LauncherE3_Definitive_Mod_Demo_Launcher  5   	   ;    5E3_Definitive_Mod_Demo_Launcher.DyingLight2Downgrader   5   	   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ?local-clr-namespace:E3_Definitive_Mod_Demo_Launcher     5   	   -   5          � Title$  DyingLight2Downgrader��5   	   $	��323��6'   $	��888��64   
���   � � WindowStyle$
 None=�6S    � 
ResizeMode$ CanMinimize=�6f   .��5
      � $��
-1,25,-2,0q�6    � ColumnDefinitions 5   
   �� 5      $	��46* �6   �� 5      $	��35* �6   ��5   
   �� 5       �� ImageSource$ /library_hero.png��6    � RowDefinitions 5   
   �� 5      $��0* �6   �� 5      .+�5   
   �� $(��"1.12.1 AIO Installer coded by dani��6   $
��Left=�6=   $��
161,45,0,0q�6X   $	��Top=�6l   $	��209��6�   $��DIN Pro Medium)�6�   $��1��6�   $��40��6�    J� RenderTransformOrigin$ 0.378,0.876&�6�   
�������6�   �� 5   
   -   5         ��Name$ UserBox��5      $
��Left=�6#   $��	14,96,0,0q�6>    �� TextWrapping$
 Wrap=�6Q   $��Enter Steam Username��6e   $	��Top=�6�   $	��354��6�   $��DIN Pro Regular)�6�   $��1��6�   $��17��6�   ��6�   Z� �� 5   
   $��Username��6   $
��Left=�6#   $��9,76,0,0q�6>   $	��Top=�6P   $ 0.211,0.156&�6h   $��DIN Pro Medium)�6�   $��25��6�   $��1��6�   $��66��6�   
�������6�   �� 5   
   $��Password��6   $
��Left=�6#   $��	9,111,0,0q�6>   $	��Top=�6Q   $ 0.211,0.156&�6i   $��DIN Pro Medium)�6�   $��1��6�   $��25��6�   $��62��6�   
�������6�   �� 5   
   -   5        $��Start Installation��5      $
��Left=�6.   $��254,203,0,0q�6I   $	��Top=�6^   $��38��6v   $	��117��6�   $��DIN Pro Bold)�6�   
���  ��
�������$��1��6�   ��6�   Z� 6�   �� 5   
   -   5        	 ��Name$	 FindDirectory��5      $��Open File Location��6(   $
��Left=�6E   $��136,203,0,0q�6`   $	��Top=�6u   $��38��6�   $	��118��6�   $��DIN Pro Bold)�6�   
���  ��
�������$��1��6  ��6�   Z� 6  �� 5   
   -   5        $ InstallLink��5      $
��Left=�6'   $��
14,169,0,0q�6B   $
 Wrap=�6V   $��File Location Here��6j   $	��Top=�6�   $	��355��6�   $��DIN Pro Regular)�6�   $��1��6�   $��16��6�   ��6�   Z� �� 5   
   $��File Location��6   $
��Left=�6(   $��	9,150,0,0q�6C   $	��Top=�6V   $ 0.211,0.156&�6n   $��DIN Pro Medium)�6�   $��1��6�   $��25��6�   $��78��6�   
�������6�   �� 5   
   -   5        $	 Log��5      $
��Left=�6   $��	393,0,0,0q�6:   $
 Wrap=�6M   $��Center=�6a   $	��475��6|   $��DIN Pro Regular)�6�   $��1��6�   $	��238��6�   {�. 
 �� 
IsReadOnly
 . 
������� J� Opacity$ 4Q�6  $��2��6  ��6�   Z� .��6,  ��5      �� 5        �� EndPoint$ 0.5,1&�6&    �� Opacity$	 0.4Q�67    �� 
StartPoint$ 0.5,0&�6E   .��5!      �  � Color$ 	#FFB9B9B9��6#   � 5"      $ 	#FF4C4C4C��6#    � Offset$ 1Q�65   5#      .��5$   "   5%      � 5&   
   -   5         �Name$ ProgressBar��5&      $
��Left=�6+   $��15��6F   $��
15,252,0,0q�6R   $��2��6f   $	��Top=�6w   $	��356��6�   ��6�   Z� �� 5'   
   $��15,5,322,232q�6   $��2��6&   $+��%/7393e75e6f946d397967efae9b3716a2.png��67   $
��Fill=�6f   �� 5(   
   $!��E3 Definitive AIO Installer��6   $
��Left=�66   $��345,-25,0,0q�6Q   $	��Top=�6f   $	��156��6~   $��30��6�   $ 0.5,0.5&�6�   
�������$��DIN Pro Bold)�6�   $��2��6�   z�5)      V� 5*      .~�5+      �� �� 5,      �� 5-       �� Angle$ 0.115Q�6&   T� 5.      5/      51      �� 52   
   -   5         ��Name$ MinimizeButton��52      $��318,-14,53,13q�6(   $��	/FOLD.png��6?   $
��Fill=�6R   $��1��6a   �� 53   
   -	   5        $ CloseButton��53      $��352,-21,17,7q�6%   $��/Vector.png��6;   $
��Fill=�6P   $��1��6_   �� 54   
   -
   5        $	 Close��54      $
��Left=�6    $��351,-24,0,0q�6;   $	��Top=�6P   $��19��6h   $ 0.5,0.5&�6�   $��1��6�   ��6s   Z� ��6�   Z� ��6�   Z� z�55      V� 56      .~�57      �� �� 58       �� AngleY$ 0.265Q�6$   �� 59      T� 5:       T� Y$ 0.037Q�6)   5;      5=      �� 5>   
   -   5        $	 Minimize��5>      $��Button��6#   $
��Left=�64   $��312,-21,0,0q�6O   $	��Top=�6d   $��25��6�   $��15��6�   $��1��6�   ��6|   Z� ��6�   Z� ��6�   Z� 6�   �� 5?   
   -   5        $	 	CloseButt��5?      $��Button��6$   $
��Left=�65   $��347,-24,0,0q�6P   $	��Top=�6e   $��27��6}   $��1��6�   ��6�   Z� ��6�   Z� ��6�   Z� 6�   �� 5@   
   -   5        $!��Get Launcher 
    Location��5@      $
��Left=�6?   $��
16,203,0,0q�6Z   $��2��6n   $	��Top=�6   
���  ��
�������$��DIN Pro Bold)�6�   $��38��6�   $	��120��6�   ��6�   Z� 6  �� 5A   
   -   5        $��debug demo download��5A      $
��Left=�6/   $��	5,311,0,0q�6J   $��2��6]   $	��Top=�6n   $��33��6�   $	��150��6�   6�   G� 5B   
   -   5         G�Name$ PassBox��5B      $
��Left=�6'   $��
14,130,0,0q�6B   $��2��6V   $	��Top=�6g   $	��354��6   $��17��6�    G� Password$  ��6�   ��6�   Z� 6�   �� 5C   
   -   5         ��Name$ 	truefalse��5C      $��Label��6#   $
��Left=�63   $��312,-73,0,0q�6N   $	��Top=�6c   ���	#FF00FF3E��6{   Z� 6�   7  1System.Windows.Forms.Integration.WindowsFormsHost  5D   
   $
��Left=�6   $��16��66   $��	193,0,0,0q�6B   $��2��6U   $	��Top=�6f   $��39��6~   �� 5E   
   -   5         ��Name$ SteamGuardCheckbox��5E      $
��Left=�6/   $��195,186,0,0q�6J   $��2��6_   $	��Top=�6p   
�������.��5F      �� -   5         ��Name$ ComboBox_AuthType��5F      $��18��6h   $��DIN Pro Medium)�6t   $��9(�6�   $	��136��6�   .@�5G      �� $��Steam Guard (Email Code)��6    �� 
IsSelected . 6B   �� 5H      $��Steam App Code��6   5I      5K      5M      5N      