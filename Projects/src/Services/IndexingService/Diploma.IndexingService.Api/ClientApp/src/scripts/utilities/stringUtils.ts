export function format(formatStr: string, ...params: any[]) {
    return formatStr.replace(/{(\d+)}/g, (match, number) => { 
      return typeof params[number] != 'undefined'
        ? params[number] 
        : match;
    });
}