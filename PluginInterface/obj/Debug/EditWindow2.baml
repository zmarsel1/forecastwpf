   M S B A M L   `   `   `  ����J  FPluginInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PluginInterfacePluginInterface  5   	   !    PluginInterface.EditWindow2   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   >d2http://schemas.microsoft.com/expression/blend/2008    5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   )localclr-namespace:PluginInterface  5   	   -   5         	  Title  � Title#  �   5   	   $	��350��5
   	   $	��600��6   $	��611��5   	   $	��600��6   $��14(�6"    � SizeToContent$ WidthAndHeight=�60    � 	Resources 5      #   PluginInterface.CrossDocument  Data��%   &	         5   
   6   5      .��5      � .+�5   
   �� -   5        $��docInfo��5   O     Info#��[ 6   $��67��60   $��	5,0,158,0q�6<   $	��Top=�6^   )  #Microsoft.Windows.Controls.DataGrid  5   
   -   5        $��gridData��5       � DataContext# �  5      $��	0,73,0,34q�5         AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .  $��Tahoma)�5         SelectionMode$ Single=�5   <   ��5      �� *Body+   	CellStyle 5      �� 5      	 �� 
TargetType-  'Microsoft.Windows.Controls.DataGridCell"	  6   .�5      -   5        
 �� Triggers
 5      P� 5       P� Property �@ 
IsSelected P� Value . . �5       ��  �� Property � �� �� Value
 ��ح�6;   �� 5!       � ��
 �   �6;   �� 5"       � ��
 ��ح�6<   5#      5%      5'      �� 5(   
   -   5        $��
chkCascade��5(   _   $-��'Каскадное обновление��6   $
��Left=�62   $��7,0,0,12q�6M   $��16��6q   $��Bottom=�6}   $	��169��6�    d� 	IsChecked$
 TrueY�6�   �� 5)   
   -   5        $��btnSave��5)   6   $��Сохранить��6   $��0,0,3,5q�6%   $��Right=�6E   $��90��6a   $��27��6l   $��Bottom=�6x   6�   �� 5*   
   -   5        $��btnClose��5*   5   $��Закрыть��6   $��0,0,99,5q�6#   $��Right=�6E   $��75��6a   $��27��6l   $��Bottom=�6x   6�   5+      5,      