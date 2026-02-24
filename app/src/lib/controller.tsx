import { type FunctionComponent, memo, type PropsWithChildren, useEffect, useRef, useState } from "react";

/**
 * Minimal Controller for managing component state and lifecycle.
 */
export abstract class Controller<
  TState extends Record<string, unknown> = {},
  TProps extends Record<string, unknown> = {},
> {
  state: TState = {} as TState;
  props: TProps = {} as TProps;

  #initiated = false;
  #destroyed = false;

  #setState: (state: Partial<TState>) => void;

  declare readonly $state: TState;
  declare readonly $props: TProps;

  constructor(setState: (state: Partial<TState>) => void) {
    this.#setState = setState;
  }

  /**
   * Factory method to create a new controller instance.
   */
  static make<TController extends typeof Controller>(
    this: TController,
    setState: any,
    setLoading: any,
    setError: any,
  ): InstanceType<TController> {
    // biome-ignore lint/complexity/noThisInStatic: should return new instance of child class
    return new (this as any)(setState, setLoading, setError) as InstanceType<TController>;
  }

  /*
   |--------------------------------------------------------------------------------
   | Lifecycle
   |--------------------------------------------------------------------------------
   */

  async $init(props: TProps): Promise<void> {
    if (this.#destroyed === true) {
      return;
    }
    this.props = props;
    if (this.onInit === undefined) {
      return;
    }
    const state = await this.onInit();
    if (this.#destroyed === false && state !== undefined) {
      this.setState(state);
    }
    this.#initiated = true;
  }

  async $resolve(props: TProps): Promise<void> {
    if (this.#initiated === false || this.#destroyed === true) {
      return;
    }
    this.props = props;
    if (this.onResolve === undefined) {
      return;
    }
    await this.onResolve();
  }

  async $destroy(): Promise<void> {
    this.#destroyed = true;
    await this.onDestroy();
  }

  /*
   |--------------------------------------------------------------------------------
   | Lifecycle Hooks
   |--------------------------------------------------------------------------------
   */

  /**
   * Called every time props change (including first mount).
   */
  async onInit(): Promise<Partial<TState> | void> {}

  /**
   * Called every time props change (including first mount).
   */
  async onResolve(): Promise<void> {}

  /**
   * Called when the controller is destroyed.
   */
  async onDestroy(): Promise<void> {}

  /*
   |--------------------------------------------------------------------------------
   | State Management
   |--------------------------------------------------------------------------------
   */

  /**
   * Updates the controller state.
   */
  setState(state: Partial<TState>): void;
  setState<K extends keyof TState>(key: K, value: TState[K]): void;
  setState<K extends keyof TState>(...args: [K | Partial<TState>, TState[K]?]): void {
    if (this.#destroyed) {
      return;
    }

    const [target, value] = args;

    if (typeof target === "string") {
      this.state = { ...this.state, [target]: value };
    } else {
      this.state = { ...this.state, ...(target as Partial<TState>) };
    }

    this.#setState(this.state);
  }

  /*
   |--------------------------------------------------------------------------------
   | Actions
   |--------------------------------------------------------------------------------
   */

  /**
   * Returns all public methods as bound actions.
   */
  toActions(): ControllerActions<this> {
    const actions: any = {};
    const prototype = Object.getPrototypeOf(this);

    for (const name of Object.getOwnPropertyNames(prototype)) {
      if (this.#isAction(name)) {
        const method = (this as any)[name];
        if (typeof method === "function") {
          actions[name] = method.bind(this);
        }
      }
    }

    return actions;
  }

  #isAction(name: string): boolean {
    return name !== "constructor" && !name.startsWith("$") && !name.startsWith("_") && !name.startsWith("#");
  }
}

/*
 |--------------------------------------------------------------------------------
 | Component
 |--------------------------------------------------------------------------------
 */

export function makeControllerComponent<TController extends new (...args: any[]) => Controller<any, any>>(
  ControllerClass: TController,
  Component: FunctionComponent<
    PropsWithChildren<
      InstanceType<TController>["$props"] &
        InstanceType<TController>["$state"] &
        ControllerActions<InstanceType<TController>>
    >
  >,
  LoadingComponent?: FunctionComponent<PropsWithChildren>,
  ErrorComponent?: FunctionComponent<PropsWithChildren<{ error: Error }>>,
): FunctionComponent<PropsWithChildren<InstanceType<TController>["$props"]>> {
  const container: FunctionComponent<PropsWithChildren> = (props: any) => {
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<any>();
    const [state, setState] = useState<any>();

    const controller = useRef<InstanceType<TController> | null>(null);
    const actions = useRef<ControllerActions<InstanceType<TController>> | null>(null);

    // biome-ignore lint/correctness/useExhaustiveDependencies: should only execute once
    useEffect(() => {
      const instance = (ControllerClass as any).make(setState);

      controller.current = instance;
      actions.current = instance.toActions();

      instance
        .$init(props || {})
        .then(() => {
          setLoading(false);
        })
        .catch((error: unknown) => {
          setError(error);
          setLoading(false);
        });

      return () => {
        instance.$destroy();
      };
    }, []);

    useEffect(() => {
      controller.current?.$resolve(props || {}).catch((error) => {
        setError(error);
      });
    }, [props]);

    if (loading === true || state === undefined) {
      return LoadingComponent ? <LoadingComponent {...props} /> : null;
    }

    if (error !== undefined) {
      return ErrorComponent ? <ErrorComponent {...props} error={error} /> : null;
    }

    return <Component {...props} {...state} {...actions.current} />;
  };

  container.displayName = `${ControllerClass.name}Component`;

  return memo(container);
}

/*
 |--------------------------------------------------------------------------------
 | Types
 |--------------------------------------------------------------------------------
 */

type ControllerActions<T> = {
  [K in keyof T]: T[K] extends (...args: any[]) => any
    ? K extends `$${string}` | `_${string}` | `#${string}` | "constructor"
      ? never
      : T[K]
    : never;
};
