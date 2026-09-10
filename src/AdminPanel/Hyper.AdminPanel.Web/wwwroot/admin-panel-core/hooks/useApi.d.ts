import { UseQueryOptions, UseMutationOptions, QueryKey } from '@tanstack/react-query';
import { ApiResponse, PaginatedResponse, QueryParams } from '../types';

/**
 * Generic API fetch hook
 */
export declare function useApiQuery<T>(key: QueryKey, url: string, params?: Record<string, unknown>, options?: Omit<UseQueryOptions<ApiResponse<T>, Error>, 'queryKey' | 'queryFn'>): import('@tanstack/react-query').UseQueryResult<ApiResponse<T>, Error>;
/**
 * Paginated API fetch hook
 */
export declare function usePaginatedQuery<T>(key: QueryKey, url: string, queryParams?: QueryParams, options?: Omit<UseQueryOptions<PaginatedResponse<T>, Error>, 'queryKey' | 'queryFn'>): import('@tanstack/react-query').UseQueryResult<PaginatedResponse<T>, Error>;
/**
 * API mutation hook (POST, PUT, PATCH, DELETE)
 */
export declare function useApiMutation<TData, TVariables = unknown>(url: string, method?: 'post' | 'put' | 'patch' | 'delete', options?: Omit<UseMutationOptions<ApiResponse<TData>, Error, TVariables>, 'mutationFn'>): import('@tanstack/react-query').UseMutationResult<ApiResponse<TData>, Error, TVariables, unknown>;
/**
 * CRUD hooks factory
 */
export declare function useCrudApi<T extends {
    id: number | string;
}>(baseUrl: string, queryKey: string): {
    useList: (params?: QueryParams) => import('@tanstack/react-query').UseQueryResult<PaginatedResponse<T>, Error>;
    useOne: (id: number | string, options?: Omit<UseQueryOptions<ApiResponse<T>, Error>, "queryKey" | "queryFn">) => import('@tanstack/react-query').UseQueryResult<ApiResponse<T>, Error>;
    useCreate: (options?: UseMutationOptions<ApiResponse<T>, Error, Partial<T>>) => import('@tanstack/react-query').UseMutationResult<ApiResponse<T>, Error, Partial<T>, unknown>;
    useUpdate: (options?: UseMutationOptions<ApiResponse<T>, Error, {
        id: number | string;
        data: Partial<T>;
    }>) => import('@tanstack/react-query').UseMutationResult<ApiResponse<T>, Error, {
        id: number | string;
        data: Partial<T>;
    }, unknown>;
    useDelete: (options?: UseMutationOptions<ApiResponse<void>, Error, number | string>) => import('@tanstack/react-query').UseMutationResult<ApiResponse<void>, Error, string | number, unknown>;
    invalidate: () => void;
};
/**
 * Hook to prefetch data
 */
export declare function usePrefetch(): <T>(key: QueryKey, url: string, params?: Record<string, unknown>) => Promise<void>;
/**
 * Hook for infinite scroll/pagination
 */
export declare function useInfiniteApi<T>(key: QueryKey, url: string, pageSize?: number): import('@tanstack/react-query').UseQueryResult<PaginatedResponse<T>, Error>;
export default useApiQuery;
