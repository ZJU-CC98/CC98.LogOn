using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// ��ʾһ��Ӧ�õ���Ϣ��
	/// </summary>
	public class App
	{
		/// <summary>
		/// ��ȡ�����øö���ı�ʶ��
		/// </summary>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// ��ȡ�����øö���Ļ��ܡ�
		/// </summary>
		public Guid Secret { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "DisplayNameName")]
		public string DisplayName { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ�������
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Display(Name = "DescriptionName")]
		public string Description { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ����صĽ�����ַ��
		/// </summary>
		[DataType(DataType.Url)]
		[Display(Name = "WebPageUriName")]
		public string WebPageUri { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ� LOGOggi ��ͼ���ַ��
		/// </summary>
		[DataType(DataType.ImageUrl)]
		[Display(Name = "LogoUriName")]
		public string LogoUri { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ�ÿͻ������Ƿ񱻽��á�
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ�������������ض����ַ�����
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(RedirectUris))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string RedirectUrisValue { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ���������ض����ַ����ļ��ϡ�
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] RedirectUris
		{
			get { return RedirectUrisValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); }
			set { RedirectUrisValue = string.Join("\n", value); }
		}

		/// <summary>
		/// ��ȡ�����øÿͻ����������������
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(AllowedScopes))]
		public string AllowedScopesValue { get; set; }

		[IgnoreDataMember]
		[NotMapped]
		public string[] AllowedScopes { get; set; }
	}

	[ComplexType]
	public class EntityStringColllection : IList<string>, IReadOnlyList<string>
	{
		/// <summary>
		/// ��ȡ���ڴ��ֵ���ڲ��ַ�����
		/// </summary>
		public string Value { get; set; }

		private Collection<string> InternalCollection { get; } = new Collection<string>();

		/// <inheritdoc />
		public IEnumerator<string> GetEnumerator()
		{
			return InternalCollection.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)InternalCollection).GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(string item)
		{
			InternalCollection.Add(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			InternalCollection.Clear();
		}

		/// <inheritdoc />
		public bool Contains(string item)
		{
			return InternalCollection.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(string[] array, int arrayIndex)
		{
			InternalCollection.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(string item)
		{
			return InternalCollection.Remove(item);
		}

		/// <inheritdoc />
		public int Count
		{
			get { return InternalCollection.Count; }
		}

		/// <inheritdoc />
		public bool IsReadOnly
		{
			get { return ((ICollection<string>)InternalCollection).IsReadOnly; }
		}

		/// <inheritdoc />
		public int IndexOf(string item)
		{
			return InternalCollection.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, string item)
		{
			InternalCollection.Insert(index, item);
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			InternalCollection.RemoveAt(index);
		}

		/// <inheritdoc />
		public string this[int index]
		{
			get { return InternalCollection[index]; }
			set { InternalCollection[index] = value; }
		}
	}
}