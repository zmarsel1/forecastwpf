   M S B A M L   `   `   `  ����   Core    Core.CreateDocument   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5c-http://schemas.microsoft.com/wpf/2008/toolkit  :d2http://schemas.microsoft.com/expression/blend/2008  -     � Title$%  Создать Документ��$	��224��$	��385�� � 
ResizeMode$ NoResize=�$��14(� � WindowStartupLocation$ Manual=� � 	Resources ��%    .��� $	��187��.+��� $ ��Тип документа:��$��27��$
��Left=�$��	12,12,0,0q�$	��Top=�$	��108���� -   $��
cmbDocType��$��27��$��
126,12,5,0q�$	��Top=� �� DisplayMemberPath$ @caption�� �� SelectedValuePath$ @type�� �� 
IsReadOnly . ����    Permissions �� Source# [   �� Mode$ OneWay=��� $��Объект:��$��27��$
��Left=�$��	59,41,0,0q�$	��Top=��� -   $��cmbDocObject��$��27��$��
126,41,5,0q�$	��Top=�$ @caption��$ @type�� . $	 �� IsSynchronizedWithCurrentItem$
	 TrueY�
 � DataContext
 ��  �� ElementName$ 
cmbDocType�� �� Path$ SelectedItem�$ OneWay=�����  �� XPath$ node[@permission='EDIT']��$ OneWay=�+  %Microsoft.Windows.Controls.DatePicker  -   $��
dtpDocDate��$��
126,70,5,0q�$��27��$	��Top=��� $"��Дата документа:��$��26��$
��Left=�$��2,69,0,0q�$	��Top=��� -   $��	btnCreate��$��Создать��$��	0,157,5,0q�$��Right=�$��75��$��27��$	��Top=��� -   $��	btnCancel��$��Отмена��$��
0,157,87,0q�$��Right=�$��72��$��27��$	��Top=��� -   $��btnAssociated��$	��...��$��	0,124,5,0q�$��Right=�$��36��$��27��$	��Top=�{��� $ chkAssociated��$ 	IsChecked��� -   $��txtParentDocument��{�.  $��27��$��126,124,47,0q�$	��Top=��� -	   $��chkAssociated��$)��#Связанный документ��$��16��$
��Left=�$��126,102,0,0q�$	��Top=�