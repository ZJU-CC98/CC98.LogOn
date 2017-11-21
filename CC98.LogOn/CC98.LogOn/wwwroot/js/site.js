/**
 * 获取 XHR 请求的错误消息。
 * @param {XMLHttpRequest} xhr 表示请求的 XHR 对象。
 * @returns 错误对象对应的错误消息。
 */
function getErrorMessage(xhr) {
	if (xhr.responseText && xhr.responseText !== '') {
		return responseText;
	}

	switch (xhr.status) {
	case 400:
		return '你提供的信息无效，请修改后再试一次。如果多次发生该错误，请联系管理员。';
	case 401:
	case 403:
		return '你没有权限执行该操作。请先登录或使用具有权限的账号重新登录。';
	default:
		return '目前无法执行该操作，请稍后再试一次。如果多次发生该错误，请联系管理员。';
	}
}

/**
 * 用于处理 AJAX 错误的通用函数。
 * @param {XMLHttpRequest} xhr 保存 AJAX 错误信息的 XHR 对象。
 * @param {JQuery} selector 用于显示 AJAX 错误的元素选择器。
 */
function handleAjaxFailure(xhr, selector) {
	$(selector).text(getErrorMessage(xhr));
}