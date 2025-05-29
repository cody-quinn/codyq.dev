document.querySelectorAll("span[data-timestamp]").forEach((el) => {
  const timestamp = el.getAttribute("data-timestamp");
  const datetime = new Date(1000 * timestamp);
  const format = el.getAttribute("data-timestamp-format");

  if (!el.hasAttribute("title")) {
    el.setAttribute("title", el.innerText);
  }

  if (format) {
    const date = datetime.toLocaleDateString();
    const time = datetime.toLocaleTimeString();
    el.innerText = format.replaceAll("{D}", date).replaceAll("{T}", time);
  } else {
    el.innerText = datetime.toLocaleString();
  }
})
