   M S B A M L   `   `   `  ����  Core    Core.CreateDocument   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   	   -   5          � Title$%  Создать Документ��5   	   $	��224��5	   	   $	��385��5
   	    � 
ResizeMode$ NoResize=�5   	   $��14(�6    � WindowStartupLocation$ Manual=�6-    � 	Resources 5      ��%    5      .��5      � $	��187��6   .+�5   
   �� $ ��Тип документа:��6   $��27��6)   $
��Left=�65   $��	12,12,0,0q�6P   $	��Top=�6c   $	��108��6{   �� 5   
   -   5        $��
cmbDocType��5      $��27��5      $��
126,12,5,0q�6   $	��Top=�5   %    �� DisplayMemberPath$ @caption��5       �� SelectedValuePath$ @type��5       �� 
IsReadOnly . ��5      ��    Permissions �� Source# [   �� Mode$ OneWay=�5      �� 5   
   $��Объект:��6   $��27��6"   $
��Left=�6.   $��	59,41,0,0q�6I   $	��Top=�6\   �� 5   
   -   5        $��cmbDocObject��5      $��27��5      $��
126,41,5,0q�6   $	��Top=�5      $ @caption��5      $ @type��5       . $	 �� IsSynchronizedWithCurrentItem$
	 TrueY�5      
 � DataContext
 5      ��  �� ElementName$ 
cmbDocType�� �� Path$ SelectedItem�$ OneWay=���5       �� 5!       �� XPath$ node[@permission='EDIT']��6   $ OneWay=�6;   5#      +  %Microsoft.Windows.Controls.DatePicker  5$   
   -   5        $��
dtpDocDate��5$      $��
126,70,5,0q�6)   $��27��6=   $	��Top=�6I   6c   �� 5%   
   $"��Дата документа:��6   $��26��6*   $
��Left=�66   $��2,69,0,0q�6Q   $	��Top=�6c   �� 5&   
   -   5        $��	btnCreate��5&   6   $��Создать��6   $��	0,157,5,0q�6#   $��Right=�6G   $��75��6c   $��27��6�   $	��Top=�6�   �� 5'   
   -   5        $��	btnCancel��5'   6   $��Отмена��6   $��
0,157,87,0q�6"   $��Right=�6G   $��72��6c   $��27��6�   $	��Top=�6�   �� 5(   
   -   5        $��btnAssociated��5(   r   $	��...��6Q   $��	0,124,5,0q�6_   $��Right=�6�   $��36��6�   $��27��6�   $	��Top=�6�   {�6   �� $ chkAssociated��$ 	IsChecked�6�   �� 5)   
   -   5        $��txtParentDocument��5)      {�.  $��27��6=   $��126,124,47,0q�6I   $	��Top=�6_   �� 5*   
   -	   5        $��chkAssociated��5*   l   $)��#Связанный документ��6   $��16��60   $
��Left=�6<   $��126,102,0,0q�6W   $	��Top=�6�   5+      5,      