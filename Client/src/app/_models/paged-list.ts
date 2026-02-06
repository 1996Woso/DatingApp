export interface PagedList {
  items(
    pageSize: number,
    totalPages: number,
    totalCount: number,
    currentPage: number
  ): string;

  pages(
    currentPage: number,
    totalPages: number
  ): string;
}
