
class Group<T> {
  key: string;
  members: T[] = [];
  constructor(key: string) {
    this.key = key;
  }
}

export class ArrayExtension {

  static groupBy<T>(list: T[], func: (x: T) => string): Group<T>[] {
    let res: Group<T>[] = [];
    let group: Group<T> = null;
    list.forEach((o) => {
      let groupName = func(o);
      if (group === null) {
        group = new Group<T>(groupName);
      }
      if (groupName != group.key) {
        res.push(group);
        group = new Group<T>(groupName);
      }
      group.members.push(o)
    });
    if (group != null) {
      res.push(group);
    }
    return res
  }
}
