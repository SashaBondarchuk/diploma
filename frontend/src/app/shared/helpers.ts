export function formatDateWithOffset(date: Date, offsetHours: number) {
  const offsetMilliseconds = offsetHours * 60 * 60 * 1000;

  const targetTime = date.getTime() + offsetMilliseconds;
  const tempDate = new Date(targetTime);

  const year = tempDate.getUTCFullYear();
  const month = String(tempDate.getUTCMonth() + 1).padStart(2, '0');
  const day = String(tempDate.getUTCDate()).padStart(2, '0');
  const hours = String(tempDate.getUTCHours()).padStart(2, '0');
  const minutes = String(tempDate.getUTCMinutes()).padStart(2, '0');
  const seconds = String(tempDate.getUTCSeconds()).padStart(2, '0');
  const milliseconds = String(tempDate.getUTCMilliseconds()).padStart(3, '0');

  const offsetSign = offsetHours >= 0 ? '+' : '-';
  const absOffsetHours = Math.abs(offsetHours);
  const offsetHoursFormatted = String(Math.floor(absOffsetHours)).padStart(
    2,
    '0'
  );
  const offsetMinutesPart = String(
    Math.round((absOffsetHours % 1) * 60)
  ).padStart(2, '0');
  const offsetString = `${offsetSign}${offsetHoursFormatted}:${offsetMinutesPart}`;

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${milliseconds}${offsetString}`;
}

export function getOffsetHoursFromString(dateString: string): number {
  if (!dateString || typeof dateString !== 'string') {
    throw new Error('Invalid date string provided for offset parsing.');
  }

  const offsetRegex = /(?:GMT|UTC)?([+-])(\d{2}):?(\d{2})($|\s)/;
  const match = dateString.match(offsetRegex);

  if (match) {
    const sign = match[1];
    const hours = parseInt(match[2], 10);
    const minutes = parseInt(match[3], 10);

    let totalHours = hours + minutes / 60.0;

    if (sign === '-') {
      totalHours = -totalHours;
    }
    console.log(
      `Parsed offset from string: ${sign}${match[2]}${match[3]} -> ${totalHours} hours`
    );
    return totalHours;
  } else {
    console.warn(
      'Could not parse a standard offset (e.g., GMT+HHMM, +/-HH:MM) from string:',
      dateString
    );
    throw new Error('Invalid date string format for offset parsing.');
  }
}
